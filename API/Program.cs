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

        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();

        builder.Services.AddScoped<IAdminService, AdminService>();
        builder.Services.AddScoped<IAdminRepository, AdminRepository>();

        builder.Services.AddScoped<IMemberService, MemberService>();
        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<IMemberRepository, MemberRepository>();

        builder.Services.AddScoped<ILikesService, LikesService>();
        builder.Services.AddScoped<ILikesRepository, LikesRepository>();

        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();

        builder.Services.AddScoped<UserActivity>();

        builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

        builder.Services.AddMemoryCache();
        builder.Services.AddResponseCaching();
        builder.Services.AddControllers().AddJsonOptions(config =>
        {
            config.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
        builder.Services.AddCors(policy =>
        {
            policy.AddPolicy("OpenCorsPolicy", options =>
                options
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
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
                        .AddDbContextCheck<DataContext>("Database Health Check")
                        .AddSqlServer(builder.Configuration.GetConnectionString("Default"));

        builder.Services.AddHealthChecksUI(options =>
        {
            options.AddHealthCheckEndpoint("WebAPI", "/health");
            options.SetEvaluationTimeInSeconds(60);
            options.SetMinimumSecondsBetweenFailureNotifications(600);
        }).AddInMemoryStorage();

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
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseResponseCaching();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        }).AllowAnonymous();
        app.MapHealthChecksUI().AllowAnonymous();
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
            UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();
            RoleManager<AppRole> roleManager = services.GetRequiredService<RoleManager<AppRole>>();
            await context.Database.MigrateAsync();
            await SeedData.SeedUsersAsync(logger, userManager, roleManager);
            await SeedData.SeedUserLikesAndMessages(logger, context);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occured during migration.");
            Console.ReadKey();
        }
    }
}
