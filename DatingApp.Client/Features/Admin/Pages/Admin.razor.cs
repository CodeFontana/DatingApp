using DatingApp.Client.Features.Admin.Components;

namespace DatingApp.Client.Features.Admin.Pages;

public partial class Admin
{
    private List<UserWithRolesResponse> _users = new();
    private List<string> _roles = new();
    private bool _loadingUsers = false;
    private string _searchString = "";
    private bool _showError = false;
    private string _errorText = "";

    [Inject] public required IAdminService AdminService { get; set; }
    [Inject] public required IDialogService DialogService { get; set; }
    [Inject] public required ISnackbar Snackbar { get; set; }

    public required MudMessageBox ConfirmDeleteBox { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadUserRolesAsync();
        await LoadUsersWithRolesAsync();
    }

    private async Task LoadUserRolesAsync()
    {
        ApiResponse<IEnumerable<string>> result = await AdminService.GetRolesAsync();

        if (result.Success)
        {
            _showError = false;
            _roles = result.Data?.ToList() ?? [];
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
        ApiResponse<IEnumerable<UserWithRolesResponse>> result = await AdminService.GetUsersWithRolesAsync();

        if (result.Success)
        {
            _showError = false;
            _users = result.Data?.ToList() ?? [];
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }

        _loadingUsers = false;
    }

    private async Task HandleEditRoles(UserWithRolesResponse selectedUser)
    {
        if (_roles.Count == 0)
        {
            Snackbar.Add("Unable to load user roles from database, unable to edit roles at this time", Severity.Error);
        }

        var parameters = new DialogParameters { ["User"]=selectedUser, ["AvailableRoles"]=_roles };
        await DialogService.ShowAsync<EditRolesDialog>($"Edit roles for {selectedUser.Username}", parameters);
    }

    private async Task HandleDeleteUser(string username)
    {
        bool? lastChance = await DialogService.ShowMessageBoxAsync("Warning", "Deleting can not be undone!", yesText: "Delete!", cancelText: "Cancel");

        if (lastChance == null)
        {
            return;
        }

        ApiResponse<bool> result = await AdminService.DeleteAccountAsync(username);

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

    private bool UserFilterFunc(UserWithRolesResponse user)
    {
        return UserFilter(user, _searchString);
    }

    private bool UserFilter(UserWithRolesResponse user, string searchString)
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
