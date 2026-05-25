namespace DatingApp.Client.Features.Members.Components;

public partial class MemberCard : IDisposable
{
    [Inject] public required NavigationManager NavManager { get; set; }
    [Inject] public required IPhotoService PhotoService { get; set; }
    [Inject] public required ILikesService LikesService { get; set; }
    [Inject] public required ISnackbar Snackbar { get; set; }
    [Inject] public required IPresenceService PresenceService { get; set; }
    [Parameter] public required MemberResponse Member { get; set; }

    private string _photoFilename = "./assets/user.png";

    protected override void OnInitialized()
    {
        PresenceService.OnlineUsersChanged += StateHasChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        _photoFilename = await PhotoService.GetPhotoAsync(Member.Username, Member.MainPhotoFilename);
        Member.MainPhotoFilename = _photoFilename;
        await base.OnParametersSetAsync();
    }

    private void HandleUserClick()
    {
        NavManager.NavigateTo($"/member/{Member.Username}");
    }

    private async Task HandleLikeToggleAsync()
    {
        ApiResponse<string> result = await LikesService.ToggleLikeAsync(Member.Username);

        if (result.Success)
        {
            Snackbar.Add(result.Data ?? "Like updated", Severity.Success);
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

    public void Dispose()
    {
        PresenceService.OnlineUsersChanged -= StateHasChanged;
    }
}
