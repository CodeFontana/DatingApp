namespace Client.Pages;

public partial class MemberDetail : IDisposable
{
    [Inject] IMemberService MemberService { get; set; }
    [Inject] IPhotoService PhotoService { get; set; }
    [Inject] ILikesService LikesService { get; set; }
    [Inject] IMessageService MessageService { get; set; }
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
    private List<MessageModel> _messages = new();
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
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        switch (StartTab)
        {
            case "about":
                await ActivateTab(_aboutTab);
                await ActivatePanel(_aboutPanel);
                break;

            case "interests":
                await ActivateTab(_interestsTab);
                await ActivatePanel(_interestsPanel);
                break;

            case "photos":
                await ActivateTab(_photosTab);
                await ActivatePanel(_photosPanel);
                break;

            case "messages":
                await ActivateTab(_messagesTab);
                await ActivatePanel(_messagesPanel);
                break;

            default:
                break;
        }
    }

    private async Task ActivateTab(MudTabPanel panel)
    {
        if (panel == null)
        {
            return;
        }
        else if (panel == _messagesTab)
        {
            await LoadMessages();
        }
        
        _memberDetailTabs.ActivatePanel(panel);
    }

    private async Task ActivatePanel(MudExpansionPanel panel)
    {
        if (panel == null)
        {
            return;
        }
        else if (panel == _messagesPanel)
        {
            await LoadMessages();
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

    private async Task HandleMessagesTabClick()
    {
        await LoadMessages();
    }

    private async Task LoadMessages()
    {
        ServiceResponseModel<IEnumerable<MessageModel>> result = await MessageService.GetMessageThreadAsync(Username);

        if (result.Success)
        {
            _messages = result.Data.ToList();
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }
    }

    public void Dispose()
    {
        PresenceService.OnlineUsersChanged -= StateHasChanged;
    }
}
