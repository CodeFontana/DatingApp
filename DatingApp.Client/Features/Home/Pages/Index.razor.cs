namespace DatingApp.Client.Features.Home.Pages;

public partial class Index
{
    [Inject] public required NavigationManager NavManager { get; set; }

    private void OnLogin()
    {
        NavManager.NavigateTo("/login");
    }

    private void OnRegister()
    {
        NavManager.NavigateTo("/register");
    }
}
