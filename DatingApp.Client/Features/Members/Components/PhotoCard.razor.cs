namespace DatingApp.Client.Features.Members.Components;

public partial class PhotoCard
{
    [Inject] public required IMemberStateService MemberStateService { get; set; }
    [Inject] public required IPhotoService PhotoService { get; set; }
    [Inject] public required ISnackbar Snackbar { get; set; }
    [Parameter] public required PhotoResponse Photo { get; set; }

    [Parameter] public EventCallback<string> OnImageChanged { get; set; }

    private string _photoFilename { get; set; } = "./assets/user.png";

    protected override async Task OnParametersSetAsync()
    {
        string? username = MemberStateService.Member?.Username;
        if (username is not null)
        {
            _photoFilename = await PhotoService.GetPhotoAsync(username, Photo.Filename);
        }

        await base.OnParametersSetAsync();
    }

    private async Task HandleSetMainPhotoAsync()
    {
        if (Photo.IsMain)
        {
            return;
        }

        string? username = MemberStateService.Member?.Username;
        if (username is null)
        {
            return;
        }

        ApiResponse<string> result = await PhotoService.SetMainPhotoAsync(username, Photo.Id);

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
        string? username = MemberStateService.Member?.Username;
        if (username is null)
        {
            return;
        }

        ApiResponse<string> result = await PhotoService.DeletePhotoAsync(username, Photo);

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
