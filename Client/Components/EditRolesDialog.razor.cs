namespace Client.Components;

public partial class EditRolesDialog
{
    [Inject] IAdminService AdminService { get; set; }
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
    }

    void Submit() => MudDialog.Close(DialogResult.Ok(true));
    void Cancel() => MudDialog.Cancel();
}