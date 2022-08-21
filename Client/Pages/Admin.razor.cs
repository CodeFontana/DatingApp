using static MudBlazor.CategoryTypes;

namespace Client.Pages;

public partial class Admin
{
    private List<UserWithRolesModel> _users = new();
    private bool _loadingUsers = false;
    private string _searchString = "";
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
        ServiceResponseModel<IEnumerable<UserWithRolesModel>> result = await AdminService.GetUsersWithRolesAsync();

        if (result.Success)
        {
            _showError = false;
            _users = result.Data.ToList();
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }

        _loadingUsers = false;
    }

    private bool UserFilterFunc(UserWithRolesModel user)
    {
        return UserFilter(user, _searchString);
    }

    private bool UserFilter(UserWithRolesModel user, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(user.Username) == false && user.Username.Equals(searchString.Trim('\"'), StringComparison.OrdinalIgnoreCase)) return true;
        if (user.Roles is not null && user.Roles.Any(r => r.Equals(searchString.Trim('\"'), StringComparison.OrdinalIgnoreCase))) return true;
        return false;
    }

}
