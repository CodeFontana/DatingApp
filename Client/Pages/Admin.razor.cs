namespace Client.Pages;

public partial class Admin
{
    private string _pageTitle;
    private bool _showError = false;
    private string _errorText;

    [Inject] ISnackbar Snackbar { get; set; }

    protected override void OnInitialized()
    {
        _pageTitle = "Admin Panel";
    }
}
