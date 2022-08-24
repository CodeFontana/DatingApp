namespace Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 9000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            config.SnackbarConfiguration.MaxDisplayedSnackbars = 3;
        });
        builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
        builder.Services.AddScoped<IPresenceService, PresenceService>();
        builder.Services.AddScoped<IMemberService, MemberService>();
        builder.Services.AddScoped<IMemberStateService, MemberStateService>();
        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<ILikesService, LikesService>();
        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddScoped<IAdminService, AdminService>();
        builder.Services.AddScoped<ISpinnerService, SpinnerService>();
        builder.Services.AddScoped<SpinnerHandler>();
        builder.Services.AddScoped(s =>
        {
            SpinnerHandler spinHandler = s.GetRequiredService<SpinnerHandler>();
            spinHandler.InnerHandler = new HttpClientHandler();
            return new HttpClient(spinHandler)
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            };
        });

        await builder.Build().RunAsync();
    }
}
