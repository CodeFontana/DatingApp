using Microsoft.JSInterop;

namespace Client.Features.Messages.Components;

public partial class MemberMessage : IAsyncDisposable
{
    [Inject] IConfiguration Configuration { get; set; }
    [Inject] IJSRuntime JSRuntime { get; set; }
    [Inject] IMemberStateService MemberStateService { get; set; }
    [Inject] IMessageService MessageService { get; set; }
    [Inject] IPresenceService PresenceService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Parameter] public string Username { get; set; }

    private MessageCreateModel _newMessage = new();
    private bool _showError = false;
    private string _errorText = string.Empty;
    private Action _presenceMessagesChangedHandler;
    private bool _subscribed;

    protected override async Task OnParametersSetAsync()
    {
        if (_subscribed == false)
        {
            MessageService.MessagesChanged += StateHasChanged;
            _presenceMessagesChangedHandler = HandlePresenceMessagesChanged;
            PresenceService.MessagesChanged += _presenceMessagesChangedHandler;
            _subscribed = true;
        }

        await LoadMessagesFromHub();
        await base.OnParametersSetAsync();
    }

    private async Task LoadMessagesFromHub()
    {
        try
        {
            string jwtToken = LocalStorageValueCompat.FromBrowser(
                await JSRuntime.InvokeAsync<string>("localStorage.getItem", Configuration["authTokenStorageKey"]));

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
            MessageService.Messages = result.Data;
            StateHasChanged();
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

    private void HandlePresenceMessagesChanged()
    {
        _ = LoadMessagesFromApi();
    }

    public async ValueTask DisposeAsync()
    {
        await MessageService.DisconnectAsync();
        MessageService.MessagesChanged -= StateHasChanged;
        PresenceService.MessagesChanged -= _presenceMessagesChangedHandler;
    }
}
