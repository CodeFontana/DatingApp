using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client.Pages;

public partial class Login
{
    private LoginUserModel _loginUser = new();
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

    private async Task OnLoginAsync()
    {
        _showError = false;
        _errorText = "";

        ServiceResponseModel<AuthUserModel> result = await AuthService.LoginAsync(_loginUser);

        if (result.Success)
        {
            NavManager.NavigateTo("/members");
        }
        else
        {
            _errorText = $"Login failed: {result.Message}";
            _showError = true;
        }
    }

    private void OnRegister()
    {
        NavManager.NavigateTo("/register");
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
