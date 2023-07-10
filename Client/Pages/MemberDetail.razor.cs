namespace Client.Pages;

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
    private string _errorText;


    protected override async Task OnParametersSetAsync()
    {
        ServiceResponseModel<MemberModel> result = await MemberService.GetMemberAsync(Username);

        if (result.Success)
        {
            _member = result.Data;
            _photoFilename = await PhotoService.GetPhotoAsync(_member.Username, _member.MainPhotoFilename);
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }

        PresenceService.OnlineUsersChanged += StateHasChanged;
        await base.OnParametersSetAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        switch (StartTab)
        {
            case "about":
                ActivateTab(_aboutTab);
                ActivatePanel(_aboutPanel);
                break;

            case "interests":
                ActivateTab(_interestsTab);
                ActivatePanel(_interestsPanel);
                break;

            case "photos":
                ActivateTab(_photosTab);
                ActivatePanel(_photosPanel);
                break;

            case "messages":
                ActivateTab(_messagesTab);
                ActivatePanel(_messagesPanel);
                break;

            default:
                break;
        }
    }

    private void ActivateTab(MudTabPanel panel)
    {
        if (panel == null)
        {
            return;
        }
        
        _memberDetailTabs.ActivatePanel(panel);
    }

    private void ActivatePanel(MudExpansionPanel panel)
    {
        if (panel == null)
        {
            return;
        }

        panel.Expand();
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
