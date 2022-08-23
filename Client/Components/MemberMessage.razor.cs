namespace Client.Components;

public partial class MemberMessage : IAsyncDisposable
{
    [Inject] IConfiguration Configuration { get; set; }
    [Inject] ILocalStorageService LocalStorage { get; set; }
    [Inject] IMemberStateService MemberStateService { get; set; }
    [Inject] IMessageService MessageService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Parameter] public string Username { get; set; }

    private MessageCreateModel _newMessage = new();
    private bool _showError = false;
    private string _errorText;

    protected override async Task OnInitializedAsync()
    {
        MessageService.MessagesChanged += StateHasChanged;
        await LoadMessagesFromHub();
    }

    private async Task LoadMessagesFromHub()
    {
        try
        {
            string jwtToken = await LocalStorage.GetItemAsync<string>(Configuration["authTokenStorageKey"]);

            if (string.IsNullOrWhiteSpace(jwtToken) == false)
            {
                await MessageService.ConnectAsync(jwtToken, Username);
            }
            else
            {
                await LoadMessagesFromApi();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to connect to messages hub: {e.Message}");
            await LoadMessagesFromApi();
        }
    }

    private async Task LoadMessagesFromApi()
    {
        ServiceResponseModel<List<MessageModel>> result = await MessageService.GetMessageThreadAsync(Username);

        if (result.Success)
        {
            MessageService.Messages = result.Data.ToList();
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }
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
