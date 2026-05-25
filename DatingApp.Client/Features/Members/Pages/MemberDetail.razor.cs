namespace DatingApp.Client.Features.Members.Pages;

public partial class MemberDetail : IDisposable
{
    [Inject] public required IMemberService MemberService { get; set; }
    [Inject] public required IPhotoService PhotoService { get; set; }
    [Inject] public required ILikesService LikesService { get; set; }
    [Inject] public required IPresenceService PresenceService { get; set; }
    [Inject] public required ISnackbar Snackbar { get; set; }
    [Parameter] public string Username { get; set; } = string.Empty;
    [Parameter] public string StartTab { get; set; } = string.Empty;

    private MudTabs _memberDetailTabs = null!;
    private MudTabPanel _aboutTab = null!;
    private MudTabPanel _interestsTab = null!;
    private MudTabPanel _photosTab = null!;
    private MudTabPanel _messagesTab = null!;
    private MudExpansionPanels _memberDetailExpPanel = null!;
    private MudExpansionPanel _aboutPanel = null!;
    private MudExpansionPanel _interestsPanel = null!;
    private MudExpansionPanel _photosPanel = null!;
    private MudExpansionPanel _messagesPanel = null!;
    private MemberResponse? _member;
    private string _photoFilename = "./assets/user.png";
    private bool _showError = false;
    private string _errorText = string.Empty;
    private bool _presenceSubscribed;


    protected override async Task OnParametersSetAsync()
    {
        ApiResponse<MemberResponse> result = await MemberService.GetMemberAsync(Username);

        if (result.Success && result.Data is not null)
        {
            _showError = false;
            _member = result.Data;
            _photoFilename = await PhotoService.GetPhotoAsync(_member.Username, _member.MainPhotoFilename);
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }

        if (_presenceSubscribed == false)
        {
            PresenceService.OnlineUsersChanged += StateHasChanged;
            _presenceSubscribed = true;
        }
        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        switch (StartTab)
        {
            case "about":
                await ActivateTab(_aboutTab);
                await ActivatePanelAsync(_aboutPanel);
                break;

            case "interests":
                await ActivateTab(_interestsTab);
                await ActivatePanelAsync(_interestsPanel);
                break;

            case "photos":
                await ActivateTab(_photosTab);
                await ActivatePanelAsync(_photosPanel);
                break;

            case "messages":
                await ActivateTab(_messagesTab);
                await ActivatePanelAsync(_messagesPanel);
                break;

            default:
                break;
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task ActivateTab(MudTabPanel? panel)
    {
        if (panel is null)
        {
            return;
        }

        await _memberDetailTabs.ActivatePanelAsync(panel);
    }

    private async Task ActivatePanelAsync(MudExpansionPanel? panel)
    {
        if (panel is null)
        {
            return;
        }

        await panel.ExpandAsync();
    }

    private async Task HandleLikeToggleAsync()
    {
        if (_member is null)
        {
            return;
        }

        ApiResponse<string> result = await LikesService.ToggleLikeAsync(_member.Username);

        if (result.Success)
        {
            Snackbar.Add(result.Data ?? "Like updated", Severity.Success);
        }
        else
        {
            Snackbar.Add(result.Message, Severity.Error);
        }
    }

    public void Dispose()
    {
        PresenceService.OnlineUsersChanged -= StateHasChanged;
    }
}
