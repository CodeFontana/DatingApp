namespace Client.Components;

public partial class PhotoCard
{
    [Inject] public IMemberStateService MemberStateService { get; set; }
    [Inject] public IPhotoService PhotoService { get; set; }
    [Inject] public ISnackbar Snackbar { get; set; }
    [Parameter] public PhotoModel Photo { get; set; }

    [Parameter] public EventCallback<string> OnImageChanged { get; set; }

    private string _photoFilename { get; set; } = "./assets/user.png";

    protected override async Task OnParametersSetAsync()
    {
        _photoFilename = await PhotoService.GetPhotoAsync(MemberStateService.Member.Username, Photo.Filename);
        await base.OnParametersSetAsync();
    }

    private async Task HandleSetMainPhotoAsync()
    {
        if (Photo.IsMain)
        {
            return;
        }

        ServiceResponseModel<string> result = await PhotoService.SetMainPhotoAsync(MemberStateService.Member.Username, Photo.Id);

        if (result.Success)
        {
            Snackbar.Add("Main photo updated successfully", Severity.Success);
            await OnImageChanged.InvokeAsync();
        }
        else
        {
            Snackbar.Add($"{result.Message}", Severity.Error);
        }
    }

    private async Task HandleDeletePhotoAsync()
    {
        ServiceResponseModel<string> result = await PhotoService.DeletePhotoAsync(MemberStateService.Member.Username, Photo);

        if (result.Success)
        {
            Snackbar.Add("Photo deleted successfully", Severity.Success);
            await OnImageChanged.InvokeAsync();
        }
        else
        {
            Snackbar.Add($"{result.Message}", Severity.Error);
        }
    }
}
