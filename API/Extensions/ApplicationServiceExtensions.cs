namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAccountRepository, AccountRepository>();

        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IAdminRepository, AdminRepository>();

        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        
        services.AddScoped<ILikesService, LikesService>();
        services.AddScoped<ILikesRepository, LikesRepository>();
        
        services.AddScoped<UserActivity>();
        
        services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
        
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("Default"));
        });

        return services;
    }
}