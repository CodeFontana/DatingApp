namespace DatingApp.Client.Features.Authentication.Pages;

public partial class Register
{
    [Inject] public required AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public required NavigationManager NavManager { get; set; }
    [Inject] public required IMemberService MemberService { get; set; }
    [Inject] public required IAuthenticationService AuthService { get; set; }
    [Inject] public required ISnackbar Snackbar { get; set; }

    private RegisterRequest _registerUser = new();
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    private bool _showPassword = false;
    private bool _showError = false;
    private string _errorText = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        AuthenticationState authState = await AuthStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            NavManager.NavigateTo("/members");
        }
    }

    private async Task OnRegisterAsync()
    {
        _showError = false;
        _errorText = "";

        ApiResponse<AuthResponse> regResult = await AuthService.RegisterAsync(_registerUser);

        if (regResult.Success)
        {
            NavManager.NavigateTo("/members");
        }
        else
        {
            _showError = true;
            _errorText = $"Registration failed: {regResult.Message}";
            Snackbar.Add($"Registration failed: {regResult.Message}", Severity.Error);
        }

        _registerUser = new();
    }

    private void OnCancel()
    {
        NavManager.NavigateTo("/");
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
}
