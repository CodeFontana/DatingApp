using API.Filters;
using API.Hubs;
using API.Interfaces;
using API.Middleware;
using API.Services;
using AspNetCoreRateLimit;
using ConsoleLoggerLibrary;
using DataAccessLibrary.Data;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Helpers;
using DataAccessLibrary.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API;

public class Program
{
    public static async Task Main(string[] args)
    {
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
            .AddSignInManager<SignInManager<AppUser>>()
            .AddRoleValidator<RoleValidator<AppRole>>()
            .AddEntityFrameworkStores<DataContext>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                                    Encoding.ASCII.GetBytes(
                                        builder.Configuration.GetValue<string>("Authentication:JwtSecurityKey"))),
                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.FromMinutes(10)
                            };

                            options.Events = new JwtBearerEvents
                            {
                                OnMessageReceived = context =>
                                {
                                    var accessToken = context.Request.Query["access_token"];
                                    var path = context.HttpContext.Request.Path;

                                    if (string.IsNullOrWhiteSpace(accessToken) == false && path.StartsWithSegments("/hubs"))
                                    {
                                        context.Token = accessToken;
                                    }

                                    return Task.CompletedTask;
                                }
                            };
                        });

        builder.Services.AddAuthorization(config =>
        {
            config.AddPolicy("RequireAdminRole", policy =>
            {
                policy.RequireRole("Administrator");
            });

            config.AddPolicy("ModeratePhotoRole", policy =>
            {
                policy.RequireRole("Moderator");
            });

            config.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        builder.Logging.ClearProviders();
        builder.Logging.AddConsoleLogger(builder.Configuration);

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IAdminService, AdminService>();
        builder.Services.AddScoped<IMemberService, MemberService>();
        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<ILikesService, LikesService>();
        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddScoped<UserActivity>();

        builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

        builder.Services.AddSignalR();
        builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
        builder.Services.AddSingleton<IPresenceTrackerService, PresenceTrackerService>();

        builder.Services.AddMemoryCache();
        builder.Services.AddResponseCaching();
        builder.Services.AddControllers().AddJsonOptions(config =>
        {
            config.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
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
                       .SetIsOriginAllowed(isOriginAllowed => true));
        });
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DatingApp API",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Specify JWT bearer token",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        builder.Services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new(1, 0);
            options.ReportApiVersions = true;
        });

        builder.Services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        builder.Services.AddHealthChecks()
                        .AddDbContextCheck<DataContext>("Database Health Check");

        builder.Services.Configure<IpRateLimitOptions>(
            builder.Configuration.GetSection("IpRateLimiting"));
        builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        builder.Services.AddInMemoryRateLimiting();

        WebApplication app = builder.Build();
        await ApplyDbMigrations(app);

        app.UseMiddleware<ExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
        }

        app.UseHttpsRedirection();
        app.UseCors("OpenCorsPolicy");
        app.UseCors("SignalRPolicy");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseResponseCaching();
        app.MapControllers();
        app.MapHub<PresenceHub>("/hubs/presence");
        app.MapHub<MessageHub>("/hubs/message");
        app.UseIpRateLimiting();
        app.MapHealthChecks("/health").AllowAnonymous();
        app.Run();
    }

    public static async Task ApplyDbMigrations(WebApplication app)
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
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occured during migration.");
            Console.ReadKey();
        }
    }
}
