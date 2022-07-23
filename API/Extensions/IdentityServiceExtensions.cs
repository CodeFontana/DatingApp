namespace API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddRoleValidator<RoleValidator<AppRole>>()
            .AddEntityFrameworkStores<DataContext>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = config.GetValue<string>("Authentication:JwtIssuer"),
                    ValidateAudience = true,
                    ValidAudience = config.GetValue<string>("Authentication:JwtAudience"),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(
                            config.GetValue<string>("Authentication:JwtSecurityKey"))),
                    ClockSkew = TimeSpan.FromMinutes(10)
                };
            });

        services
            .AddAuthorization(config =>
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

        return services;
    }
}
