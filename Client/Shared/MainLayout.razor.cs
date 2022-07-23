using Blazored.LocalStorage;
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
    [Inject] IMemberStateService MemberStateService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] ILocalStorageService LocalStorage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        string theme = await LocalStorage.GetItemAsync<string>("Theme");

        if (string.IsNullOrWhiteSpace(theme) 
            || string.Equals(theme, "Dark", System.StringComparison.CurrentCultureIgnoreCase))
        {
            _currentTheme = darkTheme;
        }
        else
        {
            _currentTheme = lightTheme;
        }

        MemberStateService.OnChange += StateHasChanged;
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
        MemberStateService.OnChange -= StateHasChanged;
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

    private async Task ToggleThemeAsync()
    {
        if (_currentTheme == lightTheme)
        {
            _currentTheme = darkTheme;
            await LocalStorage.SetItemAsync("Theme", "Dark");
        }
        else
        {
            _currentTheme = lightTheme;
            await LocalStorage.SetItemAsync("Theme", "Light");
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
            Black = "#000000FF",
            White = "#FFFFFFFF",

            Primary = "#E95420",
            PrimaryDarken = "#BE4013",
            PrimaryLighten = "#EE7A50",
            PrimaryContrastText = "#FFFFFFFF",

            Secondary = "#AD5276",
            SecondaryDarken = "#7C3A55",
            SecondaryLighten = "#C5839D",
            SecondaryContrastText = "#FFFFFFFF",

            Tertiary = "#5276AD",
            TertiaryDarken = "#3A557C",
            TertiaryLighten = "#839DC5",
            TertiaryContrastText = "#FFFFFFFF",

            Info = "#17A2B8",
            InfoDarken = "#107585",
            InfoLighten = "#40D1E7",
            InfoContrastText = "#FFFFFFFF",

            Success = "#38B44A",
            SuccessDarken = "#278035",
            SuccessLighten = "#69D178",
            SuccessContrastText = "#FFFFFFFF",

            Warning = "#EFB73E",
            WarningDarken = "#C78D10",
            WarningLighten = "#F4CC75",
            WarningContrastText = "#FFFFFFFF",

            Error = "#DF382C",
            ErrorDarken = "#A52219",
            ErrorLighten = "#E87067",
            ErrorContrastText = "#FFFFFFFF",

            Dark = "#772953",
            DarkDarken = "#5D2040",
            DarkLighten = "#BF4485",
            DarkContrastText = "#FFFFFFFF",

            TextPrimary = "#424242ff",
            TextSecondary = "#00000089",
            TextDisabled = "#00000060",

            ActionDefault = "#00000089",
            ActionDisabled = "#00000042",
            ActionDisabledBackground = "#0000001E",

            Background = "#FFFFFFFF",
            BackgroundGrey = "#F5F5F5FF",

            Surface = "#FFFFFFFF",

            DrawerBackground = "#FFFFFFFF",
            DrawerText = "#424242FF",
            DrawerIcon = "#F1F1F1FF",

            AppbarBackground = "#594AE2FF",
            AppbarText = "#FFFFFFFF",

            LinesDefault = "#0000001E",
            LinesInputs = "#BDBDBDFF",

            TableLines = "#E0E0E0FF",
            TableStriped = "#00000005",
            TableHover = "#0000000A",

            Divider = "#E0E0E0FF",
            DividerLight = "#000000CC",

            HoverOpacity = 0.06,

            GrayDefault = "#95A5A6",
            GrayLight = "#B4BCC2",
            GrayLighter = "#ECF0F1",
            GrayDark = "#7B8A8B",
            GrayDarker = "#343A40",

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
            Black = "#27272F",
            White = "#FFFFFFFF",

            Primary = "#4D4D74",
            PrimaryDarken = "#373753",
            PrimaryLighten = "#7777A5",
            PrimaryContrastText = "#FFFFFFFF",

            Secondary = "#4E6774",
            SecondaryDarken = "#374953",
            SecondaryLighten = "#7795A5",
            SecondaryContrastText = "#FFFFFFFF",

            Tertiary = "#4E7461",
            TertiaryDarken = "#375345",
            TertiaryLighten = "#77A58E",
            TertiaryContrastText = "#FFFFFFFF",

            Info = "#4A86FFFF",
            InfoDarken = "#2970FF",
            InfoLighten = "#70A0FF",
            InfoContrastText = "#FFFFFFFF",

            Success = "#3DCB6CFF",
            SuccessDarken = "#2FB15B",
            SuccessLighten = "#5ED485",
            SuccessContrastText = "#FFFFFFFF",

            Warning = "#FFB545FF",
            WarningDarken = "#FFA724",
            WarningLighten = "#FFC670",
            WarningContrastText = "#FFFFFFFF",

            Error = "#FF3F5FFF",
            ErrorDarken = "#FF1A40",
            ErrorLighten = "#FF6680",
            ErrorContrastText = "#FFFFFFFF",

            Dark = "#424242FF",
            DarkDarken = "#2E2E2E",
            DarkLighten = "#575757",
            DarkContrastText = "#FFFFFFFF",

            TextPrimary = "rgba(255,255,255, 0.70)",
            TextSecondary = "rgba(255,255,255, 0.50)",
            TextDisabled = "rgba(255,255,255, 0.2)",

            ActionDefault = "#ADADB1",
            ActionDisabled = "rgba(255,255,255, 0.26)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",

            Background = "#1A1A27FF",
            BackgroundGrey = "#151521FF",

            Surface = "#1E1E2DFF",

            DrawerBackground = "#1A1A27FF",
            DrawerText = "#92929FFF",
            DrawerIcon = "#92929FFF",

            AppbarBackground = "#1A1A27CC",
            AppbarText = "#92929FFF",

            LinesDefault = "#33323EFF",
            LinesInputs = "#BDBDBDFF",

            TableLines = "#33323EFF",
            TableStriped = "#00000005",
            TableHover = "#0000000A",

            Divider = "#292838FF",
            DividerLight = "#000000CC",

            HoverOpacity = 0.06,

            GrayDefault = "#9E9E9E",
            GrayLight = "#2A2833",
            GrayLighter = "#1E1E2D",
            GrayDark = "#757575",
            GrayDarker = "#616161",

            OverlayDark = "rgba(33,33,33,0.4980392156862745)",
            OverlayLight = "#1e1e2d80"
        }
    };
}
