namespace Client.Pages;

public partial class Register
{
    [Inject] AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] NavigationManager NavManager { get; set; }
    [Inject] IMemberService MemberService { get; set; }
    [Inject] IAuthenticationService AuthService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

    private RegisterUserModel _registerUser = new();
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    private bool _showPassword = false;
    private bool _showError = false;
    private string _errorText;

    protected override async Task OnInitializedAsync()
    {
        AuthenticationState authState = await AuthStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = authState.User;

        if (user.Identity.IsAuthenticated)
        {
            NavManager.NavigateTo("/members");
        }
    }

    private async Task OnRegisterAsync()
    {
        _showError = false;
        _errorText = "";

        ServiceResponseModel<AuthUserModel> regResult = await AuthService.RegisterAsync(_registerUser);

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
