using Client.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client.Shared;

public partial class MainLayout
{
    private MudTheme _currentTheme = new();
    private LoginUserModel _loginUser = new();
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    private bool _showPassword = false;
    private bool _drawerOpen = false;

    [Inject] AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] NavigationManager NavManager { get; set; }
    [Inject] IAuthenticationService AuthService { get; set; }
    [Inject] IMemberService MemberService { get; set; }
    [Inject] IAppUserService AppUserService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _currentTheme = darkTheme;

        AppUserService.OnChange += StateHasChanged;
        AuthenticationState authState = await AuthStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = authState.User;

        if (user.Identity.IsAuthenticated)
        {
            if (NavManager.Uri == NavManager.BaseUri)
            {
                NavManager.NavigateTo("/members");
            }
        }
    }

    private async Task HandleLoginAsync()
    {
        if (string.IsNullOrEmpty(_loginUser.Username))
        {
            Snackbar.Add("Please enter your username", Severity.Info);
            return;
        }
        else if (string.IsNullOrEmpty(_loginUser.Password))
        {
            Snackbar.Add("Please enter your password", Severity.Info);
            return;
        }

        ServiceResponseModel<AuthUserModel> authResult = await AuthService.LoginAsync(_loginUser);
        _loginUser = new();

        if (authResult.Success)
        {
            NavManager.NavigateTo("/members");
        }
        else
        {
            Snackbar.Add($"Login failed: {authResult.Message}", Severity.Error);
        }
    }

    private async Task HandleLogoutAsync()
    {
        NavManager.NavigateTo("/");
        await AuthService.LogoutAsync();
    }

    public void Dispose()
    {
        AppUserService.OnChange -= StateHasChanged;
    }

    private void ToggleShowPassword()
    {
        if (_showPassword)
        {
            _showPassword = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _showPassword = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }
    }

    private void ToggleThemeAsync()
    {
        if (_currentTheme == lightTheme)
        {
            _currentTheme = darkTheme;
        }
        else
        {
            _currentTheme = lightTheme;
        }
    }

    private MudTheme lightTheme = new()
    {
        Typography = new()
        {
            Default = new()
            {
                //FontFamily = new[] { "Lato", "-apple-system", "BlinkMacSystemFont", "Segoe UI", "Roboto", "Helvetica Neue", "Arial", "sans-serif", "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol" }
            }
        },
        Palette = new()
        {
            Black = "#000000ff",
            White = "#ffffffff",
            Primary = "#2c3e50",
            PrimaryContrastText = "#ffffffff",
            Secondary = "#526162",
            SecondaryContrastText = "#ffffffff",
            Tertiary = "#5E82A7",
            TertiaryContrastText = "#ffffffff",
            Info = "#3498db",
            InfoContrastText = "#ffffffff",
            Success = "#00c853ff",
            SuccessContrastText = "#ffffffff",
            Warning = "#f39c12",
            WarningContrastText = "#ffffffff",
            Error = "#e74c3c",
            ErrorContrastText = "#ffffffff",
            Dark = "#27272f",
            DarkContrastText = "#ffffffff",
            TextPrimary = "#424242ff",
            TextSecondary = "#00000089",
            TextDisabled = "#00000060",
            ActionDefault = "#00000089",
            ActionDisabled = "#00000042",
            ActionDisabledBackground = "#0000001e",
            Background = "#ffffffff",
            BackgroundGrey = "#f5f5f5ff",
            Surface = "#ffffffff",
            DrawerBackground = "#ffffffff",
            DrawerText = "#424242ff",
            DrawerIcon = "#616161ff",
            AppbarBackground = "#594ae2ff",
            AppbarText = "#ffffffff",
            LinesDefault = "#0000001e",
            LinesInputs = "#bdbdbdff",
            TableLines = "#e0e0e0ff",
            TableStriped = "#00000005",
            TableHover = "#0000000a",
            Divider = "#e0e0e0ff",
            DividerLight = "#000000cc",
            PrimaryDarken = "#253444",
            PrimaryLighten = "#3F5973",
            SecondaryDarken = "#3A4545",
            SecondaryLighten = "#728688",
            TertiaryDarken = "#3B546C",
            TertiaryLighten = "#97AFC6",
            InfoDarken = "rgb(12,128,223)",
            InfoLighten = "rgb(71,167,245)",
            SuccessDarken = "rgb(0,163,68)",
            SuccessLighten = "rgb(0,235,98)",
            WarningDarken = "rgb(214,129,0)",
            WarningLighten = "rgb(255,167,36)",
            ErrorDarken = "rgb(242,28,13)",
            ErrorLighten = "rgb(246,96,85)",
            DarkDarken = "#222229",
            DarkLighten = "#434350",
            HoverOpacity = 0.06,
            GrayDefault = "#95a5a6",
            GrayLight = "#b4bcc2",
            GrayLighter = "#ecf0f1",
            GrayDark = "#7b8a8b",
            GrayDarker = "#343a40",
            OverlayDark = "rgba(33,33,33,0.4980392156862745)",
            OverlayLight = "rgba(255,255,255,0.4980392156862745)"
        }
    };

    private MudTheme darkTheme = new()
    {
        Typography = new()
        {
            Default = new()
            {
                //FontFamily = new[] { "Lato", "-apple-system", "BlinkMacSystemFont", "Segoe UI", "Roboto", "Helvetica Neue", "Arial", "sans-serif", "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol" }
            }
        },
        Palette = new()
        {
            Black = "#27272f",
            White = "#ffffffff",
            Primary = "#594ae2ff",
            PrimaryContrastText = "#ffffffff",
            Secondary = "#4BA0E2ff",
            SecondaryContrastText = "#ffffffff",
            Tertiary = "#4E7797ff",
            TertiaryContrastText = "#ffffffff",
            Info = "#2196f3ff",
            InfoContrastText = "#ffffffff",
            Success = "#00c853ff",
            SuccessContrastText = "#ffffffff",
            Warning = "#ff9800ff",
            WarningContrastText = "#ffffffff",
            Error = "#f44336ff",
            ErrorContrastText = "#ffffffff",
            Dark = "#27272f",
            DarkContrastText = "#ffffffff",
            TextPrimary = "rgba(255,255,255, 0.70)",
            TextSecondary = "rgba(255,255,255, 0.50)",
            TextDisabled = "rgba(255,255,255, 0.2)",
            ActionDefault = "#adadb1",
            ActionDisabled = "rgba(255,255,255, 0.26)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            Background = "#1a1a27ff",
            BackgroundGrey = "#151521ff",
            Surface = "#1e1e2dff",
            DrawerBackground = "#1a1a27ff",
            DrawerText = "#92929fff",
            DrawerIcon = "#92929fff",
            AppbarBackground = "#1a1a27cc",
            AppbarText = "#92929fff",
            LinesDefault = "#33323eff",
            LinesInputs = "#bdbdbdff",
            TableLines = "#33323eff",
            TableStriped = "#00000005",
            TableHover = "#0000000a",
            Divider = "#292838ff",
            DividerLight = "#000000cc",
            PrimaryDarken = "rgb(62,44,221)",
            PrimaryLighten = "rgb(118,106,231)",
            SecondaryDarken = "#2182CB",
            SecondaryLighten = "#72B5E8",
            TertiaryDarken = "#3D5E77",
            TertiaryLighten = "#6E96B4",
            InfoDarken = "rgb(12,128,223)",
            InfoLighten = "rgb(71,167,245)",
            SuccessDarken = "rgb(0,163,68)",
            SuccessLighten = "rgb(0,235,98)",
            WarningDarken = "rgb(214,129,0)",
            WarningLighten = "rgb(255,167,36)",
            ErrorDarken = "rgb(242,28,13)",
            ErrorLighten = "rgb(246,96,85)",
            DarkDarken = "#222229",
            DarkLighten = "#434350",
            HoverOpacity = 0.06,
            GrayDefault = "#9E9E9E",
            GrayLight = "#2a2833",
            GrayLighter = "#1e1e2d",
            GrayDark = "#757575",
            GrayDarker = "#616161",
            OverlayDark = "rgba(33,33,33,0.4980392156862745)",
            OverlayLight = "#1e1e2d80"
        }
    };
}
