using Microsoft.JSInterop;

namespace DatingApp.Client.Features.Layout;

public partial class MainLayout : IDisposable
{
    private bool _isDarkMode = true;
    private bool _drawerOpen = false;

    [Inject] public required AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public required NavigationManager NavManager { get; set; }
    [Inject] public required IAuthenticationService AuthService { get; set; }
    [Inject] public required IMemberStateService MemberStateService { get; set; }
    [Inject] public required IJSRuntime JSRuntime { get; set; }
    [Inject] public required IWebAssemblyHostEnvironment HostEnv { get; set; }

    private string _themeIcon => _isDarkMode
        ? Icons.Material.Rounded.LightMode
        : Icons.Material.Rounded.DarkMode;

    private string _themeTooltip => _isDarkMode
        ? "Switch to light mode"
        : "Switch to dark mode";

    private string _environmentLabel =>
        HostEnv.IsDevelopment() ? "Development" : "Production";

    protected override async Task OnInitializedAsync()
    {
        string? theme = LocalStorageValueCompat.FromBrowser(
            await JSRuntime.InvokeAsync<string?>("localStorage.getItem", "Theme"));

        // Preserve the original DatingApp default of dark mode when the user
        // hasn't picked one explicitly.
        _isDarkMode = string.IsNullOrWhiteSpace(theme)
            || string.Equals(theme, "Dark", StringComparison.InvariantCultureIgnoreCase);

        MemberStateService.OnChange += StateHasChanged;

        AuthenticationState authState = await AuthStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = authState.User;

        if (user.Identity?.IsAuthenticated == true && NavManager.Uri == NavManager.BaseUri)
        {
            NavManager.NavigateTo("/members");
        }
    }

    private void ToggleDrawer()
    {
        _drawerOpen = !_drawerOpen;
    }

    private async Task ToggleThemeAsync()
    {
        _isDarkMode = !_isDarkMode;
        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "Theme", _isDarkMode ? "Dark" : "Light");
    }

    private async Task HandleLogoutAsync()
    {
        NavManager.NavigateTo("/");
        await AuthService.LogoutAsync();
    }

    public void Dispose()
    {
        MemberStateService.OnChange -= StateHasChanged;
    }
}
