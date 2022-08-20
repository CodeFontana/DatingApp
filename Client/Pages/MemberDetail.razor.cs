using DataAccessLibrary.Entities;
using static MudBlazor.CategoryTypes;

namespace Client.Pages;

public partial class MemberDetail
{
    [Inject] IMemberService MemberService { get; set; }
    [Inject] IPhotoService PhotoService { get; set; }
    [Inject] ILikesService LikesService { get; set; }
    [Inject] IMessageService MessageService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Parameter] public string Username { get; set; }

    private MudTabs _memberDetailTabs;
    private MudTabPanel _aboutTab;
    private MudTabPanel _interestsTab;
    private MudTabPanel _photosTab;
    private MudTabPanel _messagesTab;
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
    }

    private async Task ActivateTab(MudTabPanel panel)
    {
        if (panel == _messagesTab)
        {
            await LoadMessages();
        }
        
        _memberDetailTabs.ActivatePanel(panel);
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
}
