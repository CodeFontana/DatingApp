namespace Client.Components;

public partial class MemberCard
{
    [Inject] NavigationManager NavManager { get; set; }
    [Inject] IPhotoService PhotoService { get; set; }
    [Inject] ILikesService LikesService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Parameter] public MemberModel Member { get; set; }

    private string _photoFilename = "./assets/user.png";

    protected override async Task OnParametersSetAsync()
    {
        _photoFilename = await PhotoService.GetPhotoAsync(Member.Username, Member.MainPhotoFilename);
        Member.MainPhotoFilename = _photoFilename;
    }

    private void HandleUserClick()
    {
        NavManager.NavigateTo($"/member/{Member.Username}");
    }

    private async Task HandleLikeToggleAsync()
    {
        ServiceResponseModel<string> result = await LikesService.ToggleLikeAsync(Member.Username);

        if (result.Success)
        {
            Snackbar.Add(result.Data, Severity.Success);
        }
        else
        {
            Snackbar.Add(result.Message, Severity.Error);
        }
    }

    private void HandleMessageClick()
    {
        NavManager.NavigateTo($"/member/{Member.Username}/messages");
    }
}
