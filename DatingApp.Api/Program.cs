using System.Net.Mime;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using ConsoleLoggerLibrary;
using DatingApp.Api;
using DatingApp.Api.Features.Messages;
using DatingApp.Api.Infrastructure.Hubs;
using DatingApp.Api.Infrastructure.Middleware;
using DatingApp.DataAccess.Data;
using DatingApp.DataAccess.Entities;
using DatingApp.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddIdentityCore<AppUser>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
})
    .AddRoles<AppRole>()
    .AddRoleManager<RoleManager<AppRole>>()
    .AddRoleValidator<RoleValidator<AppRole>>()
    .AddEntityFrameworkStores<DataContext>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetValue<string>("Authentication:JwtIssuer"),
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetValue<string>("Authentication:JwtAudience"),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("Authentication:JwtSecurityKey")!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(5)
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                string? accessToken = context.Request.Query["access_token"];
                PathString path = context.HttpContext.Request.Path;

                if (string.IsNullOrWhiteSpace(accessToken) == false && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Administrator"))
    .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Moderator"))
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

builder.Logging.ClearProviders();
builder.Logging.AddConsoleLogger(builder.Configuration);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddDatingAppFeatures();

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
builder.Services.AddSingleton<IPresenceTrackerService, PresenceTrackerService>();

builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();
builder.Services.AddOutputCache();
builder.Services.ConfigureHttpJsonOptions(config =>
{
    config.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<JwtBearerSecuritySchemeTransformer>();
});
builder.Services.AddCors(policy =>
{
    policy.AddPolicy("OpenCorsPolicy", options =>
        options.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod());

    policy.AddPolicy("SignalRPolicy", options =>
        options.AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials()
               .SetIsOriginAllowed(_ => true));
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<DataContext>("Database Health Check");

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 4;
        limiterOptions.Window = TimeSpan.FromSeconds(12);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    options.OnRejected = (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = MediaTypeNames.Text.Plain;
        context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()?
            .CreateLogger("Microsoft.AspNetCore.RateLimitingMiddleware")
            .LogWarning("OnRejected: {GetUserEndPoint}", GetUserEndPoint(context.HttpContext));
        return new ValueTask(context.HttpContext.Response.WriteAsync(
            "Rate limit exceeded. Please try again later.",
            cancellationToken: cancellationToken));
    };
});

WebApplication app = builder.Build();
await ApplyDbMigrations(app);

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "DatingApp v1");
        options.EnableTryItOutByDefault();
        options.ConfigObject.AdditionalItems["syntaxHighlight"] = new Dictionary<string, object>
        {
            ["activated"] = false
        };
    });
}

app.UseHttpsRedirection();
app.UseCors("OpenCorsPolicy");
app.UseCors("SignalRPolicy");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCaching();
app.UseOutputCache();
app.MapDatingAppEndpoints();
app.MapHub<PresenceHub>("/hubs/presence");
app.MapHub<MessageHub>("/hubs/message");
app.MapHealthChecks("/health").AllowAnonymous();
app.Run();

static string GetUserEndPoint(HttpContext context) =>
    $"User {context.User.Identity?.Name ?? "Anonymous"}, " +
    $"Endpoint: {context.Request.Path}, " +
    $"IP: {context.Connection.RemoteIpAddress}";

static async Task ApplyDbMigrations(WebApplication app)
{
    using IServiceScope scope = app.Services.CreateScope();
    IServiceProvider services = scope.ServiceProvider;

    try
    {
        DataContext context = services.GetRequiredService<DataContext>();
        ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
        IUnitOfWork unitOfWork = services.GetRequiredService<IUnitOfWork>();
        await context.Database.MigrateAsync();
        await SeedData.SeedUsersAsync(logger, unitOfWork);
        await SeedData.SeedUserLikesAndMessages(logger, unitOfWork);
    }
    catch (Exception ex)
    {
        ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during migration.");
    }
}

internal sealed class JwtBearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        IEnumerable<AuthenticationScheme> authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            Dictionary<string, IOpenApiSecurityScheme> securitySchemes = new()
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = securitySchemes;

            foreach (KeyValuePair<HttpMethod, OpenApiOperation> operation in document.Paths.Values.SelectMany(path => path.Operations!))
            {
                operation.Value.Security ??= [];
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            }
        }
    }
}
