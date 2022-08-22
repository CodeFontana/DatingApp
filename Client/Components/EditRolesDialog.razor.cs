namespace Client.Components;

public partial class EditRolesDialog
{
    private string _leftValue;
    private string _rightValue;
    private List<string> _diffRoles;

    [Inject] IAdminService AdminService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [CascadingParameter] public MudDialogInstance MudDialog { get; set; }
    [Parameter] public UserWithRolesModel User { get; set; }
    [Parameter] public List<string> AvailableRoles { get; set; }

    protected override void OnInitialized()
    {
        MudDialog.Options.CloseButton = true;
        MudDialog.Options.CloseOnEscapeKey = true;
        MudDialog.Options.FullWidth = true;
        MudDialog.SetOptions(MudDialog.Options);
    }

    protected override void OnParametersSet()
    {
        MudDialog.SetTitle($"Edit roles for {User.Username}");
        _diffRoles = AvailableRoles.Except(User.Roles).ToList();
    }

    private void HandleRight()
    {
        if (string.IsNullOrWhiteSpace(_leftValue) == false)
        {
            _diffRoles.Add(_leftValue);
            User.Roles.Remove(_leftValue);
            _leftValue = null;
        }
    }

    private void HandleLeft()
    {
        if (string.IsNullOrWhiteSpace(_rightValue) == false)
        {
            User.Roles.Add(_rightValue);
            _diffRoles.Remove(_rightValue);
            _rightValue = null;
        }
    }

    private async Task Submit()
    {
        ServiceResponseModel<string> result = await AdminService.EditRolesAsync(User);

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