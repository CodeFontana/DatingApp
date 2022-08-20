namespace Client.Components;

public partial class MemberMessage
{
    [Inject] IMemberStateService MemberStateService { get; set; }
    [Inject] IMessageService MessageService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Parameter] public string Username { get; set; }
    [Parameter] public List<MessageModel> Messages { get; set; }

    private MessageCreateModel _newMessage = new();
    private bool _showError = false;
    private string _errorText;

    private async Task HandleSendMessage()
    {
        _newMessage.RecipientUsername = Username;
        ServiceResponseModel<MessageModel> result = await MessageService.CreateMessageAsync(_newMessage);

        if (result.Success)
        {
            result.Data.SenderPhotoUrl = MemberStateService.MainPhoto;
            Messages.Add(result.Data);
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
