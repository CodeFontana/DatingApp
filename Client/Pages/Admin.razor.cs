using Client.Components;

namespace Client.Pages;

public partial class Admin
{
    private List<UserWithRolesModel> _users = new();
    private List<string> _roles = new();
    private bool _loadingUsers = false;
    private string _searchString = "";
    private bool _showError = false;
    private string _errorText = "";

    [Inject] IAdminService AdminService { get; set; }
    [Inject] IDialogService DialogService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

    public MudMessageBox ConfirmDeleteBox { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadUserRolesAsync();
        await LoadUsersWithRolesAsync();
    }

    private async Task LoadUserRolesAsync()
    {
        ServiceResponseModel<IEnumerable<string>> result = await AdminService.GetRolesAsync();

        if (result.Success)
        {
            _showError = false;
            _roles = result.Data.ToList();
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }
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

    private void HandleEditRoles(UserWithRolesModel selectedUser)
    {
        if (_roles is null || _roles.Count == 0)
        {
            Snackbar.Add("Unable to load user roles from database, unable to edit roles at this time", Severity.Error);
        }
        
        var parameters = new DialogParameters { ["User"]=selectedUser, ["AvailableRoles"]=_roles };
        DialogService.Show<EditRolesDialog>($"Edit roles for {selectedUser.Username}", parameters);
    }

    private async Task HandleDeleteUser(string username)
    {
        bool? lastChance = await DialogService.ShowMessageBox("Warning", "Deleting can not be undone!", yesText: "Delete!", cancelText: "Cancel");

        if (lastChance == null)
        {
            return;
        }

        ServiceResponseModel<bool> result = await AdminService.DeleteAccountAsync(username);

        if (result.Success)
        {
            _showError = false;
            await LoadUserRolesAsync();
            await LoadUsersWithRolesAsync();
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }
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
