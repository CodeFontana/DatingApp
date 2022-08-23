namespace Client.Components;

public partial class MemberMessage : IAsyncDisposable
{
    [Inject] IMemberStateService MemberStateService { get; set; }
    [Inject] IMessageService MessageService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Parameter] public string Username { get; set; }

    private MessageCreateModel _newMessage = new();
    private bool _showError = false;
    private string _errorText;

    protected override void OnInitialized()
    {
        MessageService.MessagesChanged += StateHasChanged;
    }

    private async Task HandleSendMessageToHub()
    {
        try
        {
            _newMessage.RecipientUsername = Username;
            await MessageService.CreateHubMessageAsync(_newMessage);
            _newMessage = new();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to send hub message: {e.Message}");
            await HandleSendMessageToApi();
        }
    }
    
    private async Task HandleSendMessageToApi()
    {
        _newMessage.RecipientUsername = Username;
        ServiceResponseModel<MessageModel> result = await MessageService.CreateMessageAsync(_newMessage);

        if (result.Success)
        {
            result.Data.SenderPhotoUrl = MemberStateService.MainPhoto;
            MessageService.Messages.Add(result.Data);
            _newMessage = new();
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await MessageService.DisconnectAsync();
        MessageService.MessagesChanged -= StateHasChanged;
    }
}
