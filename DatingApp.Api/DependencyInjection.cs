using DatingApp.Api.Features.Admin;
using DatingApp.Api.Features.Authentication;
using DatingApp.Api.Features.Likes;
using DatingApp.Api.Features.Members;
using DatingApp.Api.Features.Messages;
using DatingApp.Api.Infrastructure.Authentication;
using DatingApp.Api.Infrastructure.Filters;

namespace DatingApp.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDatingAppFeatures(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<ILikesService, LikesService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<UserActivityFilter>();
        return services;
    }

    public static WebApplication MapDatingAppEndpoints(this WebApplication app)
    {
        app.MapAuthenticationEndpoints();
        app.MapMemberEndpoints();
        app.MapMessageEndpoints();
        app.MapLikesEndpoints();
        app.MapAdminEndpoints();
        return app;
    }
}
