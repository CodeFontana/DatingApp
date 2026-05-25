namespace DatingApp.Client.Features.Admin.Components;

public partial class EditRolesDialog
{
    private string _leftValue = string.Empty;
    private string _rightValue = string.Empty;
    private List<string> _diffRoles = [];

    [Inject] public required IAdminService AdminService { get; set; }
    [Inject] public required ISnackbar Snackbar { get; set; }
    [CascadingParameter] public required IMudDialogInstance MudDialog { get; set; }
    [Parameter] public required UserWithRolesResponse User { get; set; }
    [Parameter] public required List<string> AvailableRoles { get; set; }

    protected override void OnParametersSet()
    {
        _diffRoles = AvailableRoles.Except(User.Roles).ToList();
        base.OnParametersSet();
    }

    private void HandleRight()
    {
        if (string.IsNullOrWhiteSpace(_leftValue) == false)
        {
            _diffRoles.Add(_leftValue);
            User.Roles.Remove(_leftValue);
            _leftValue = string.Empty;
        }
    }

    private void HandleLeft()
    {
        if (string.IsNullOrWhiteSpace(_rightValue) == false)
        {
            User.Roles.Add(_rightValue);
            _diffRoles.Remove(_rightValue);
            _rightValue = string.Empty;
        }
    }

    private async Task Submit()
    {
        ApiResponse<string> result = await AdminService.EditRolesAsync(User);

        if (result.Success)
        {
            Snackbar.Add($"Successfully editted roles for {User.Username}", Severity.Success);
        }
        else
        {
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }

        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}
