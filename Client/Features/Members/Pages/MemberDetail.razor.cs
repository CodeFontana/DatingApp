namespace Client.Features.Members.Pages;

public partial class MemberDetail : IDisposable
{
    [Inject] IMemberService MemberService { get; set; }
    [Inject] IPhotoService PhotoService { get; set; }
    [Inject] ILikesService LikesService { get; set; }
    [Inject] IPresenceService PresenceService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Parameter] public string Username { get; set; }
    [Parameter] public string StartTab { get; set; }

    private MudTabs _memberDetailTabs;
    private MudTabPanel _aboutTab;
    private MudTabPanel _interestsTab;
    private MudTabPanel _photosTab;
    private MudTabPanel _messagesTab;
    private MudExpansionPanels _memberDetailExpPanel;
    private MudExpansionPanel _aboutPanel;
    private MudExpansionPanel _interestsPanel;
    private MudExpansionPanel _photosPanel;
    private MudExpansionPanel _messagesPanel;
    private MemberModel _member;
    private string _photoFilename = "./assets/user.png";
    private bool _showError = false;
    private string _errorText = string.Empty;
    private bool _presenceSubscribed;


    protected override async Task OnParametersSetAsync()
    {
        ServiceResponseModel<MemberModel> result = await MemberService.GetMemberAsync(Username);

        if (result.Success)
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

    private async Task ActivateTab(MudTabPanel panel)
    {
        if (panel == null)
        {
            return;
        }

        await _memberDetailTabs.ActivatePanelAsync(panel);
    }

    private async Task ActivatePanelAsync(MudExpansionPanel panel)
    {
        if (panel == null)
        {
            return;
        }

        await panel.ExpandAsync();
    }

    private async Task HandleLikeToggleAsync()
    {
        ServiceResponseModel<string> result = await LikesService.ToggleLikeAsync(_member.Username);

        if (result.Success)
        {
            Snackbar.Add(result.Data, Severity.Success);
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
