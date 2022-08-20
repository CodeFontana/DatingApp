namespace Client.Components;

public partial class MemberMessage
{
    [Inject] IMemberStateService MemberStateService { get; set; }
    [Inject] IMessageService MessageService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Parameter] public string Username { get; set; }

    private List<MessageModel> _messages = new();
    private MessageCreateModel _newMessage = new();
    private bool _showError = false;
    private string _errorText;

    protected override async Task OnParametersSetAsync()
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

    private async Task HandleSendMessage()
    {
        _newMessage.RecipientUsername = Username;
        ServiceResponseModel<MessageModel> result = await MessageService.CreateMessageAsync(_newMessage);

        if (result.Success)
        {
            result.Data.SenderPhotoUrl = MemberStateService.MainPhoto;
            _messages.Add(result.Data);
            _newMessage = new();
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }
    }
}
