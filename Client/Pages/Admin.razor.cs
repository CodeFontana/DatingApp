namespace Client.Pages;

public partial class Admin
{
    private List<UserWithRolesModel> _users = new();
    private PaginationParameters _pageParameters = new();
    private PaginationModel _metaData;
    private bool _loadingUsers = false;
    private bool _showError = false;
    private string _errorText = "";

    [Inject] IAdminService AdminService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadUsersWithRolesAsync();
    }

    private async Task LoadUsersWithRolesAsync()
    {
        _loadingUsers = true;
        PaginationResponseModel<IEnumerable<UserWithRolesModel>> result = await AdminService.GetUsersWithRolesAsync(_pageParameters);

        if (result.Success)
        {
            _showError = false;
            _users = result.Data.ToList();
            _metaData = result.MetaData;
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }

        _loadingUsers = false;
    }

    private async Task HandlePageChangedAsync(int page)
    {
        _pageParameters.PageNumber = page;
        _users = null;
        await LoadUsersWithRolesAsync();
    }
}
