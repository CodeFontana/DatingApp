namespace Client.Features.Messages.Pages;

public partial class Messages
{
    private MudTable<MessageModel> _messageTable;
    private List<MessageModel> _messages = new();
    private MessageParameters _messageFilter = new();
    private PaginationModel _metaData = new();
    private string _pageTitle = "Messages";
    private string _selectedContainer = "inbox";
    private bool _loadingMessages = false;
    private bool _showError = false;
    private string _errorText = string.Empty;

    [Inject] IMessageService MessageService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] NavigationManager NavManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(_messageFilter.Container))
        {
            _messageFilter.Container = "Inbox";
        }
        _selectedContainer = _messageFilter.Container.ToLowerInvariant();
        SetPageTitle(_selectedContainer);

        await LoadMessagesAsync();
    }

    private async Task LoadMessagesAsync()
    {
        _loadingMessages = true;
        PaginationResponseModel<List<MessageModel>> result = await MessageService.GetMessagesForMemberAsync(_messageFilter);

        if (result.Success)
        {
            _showError = false;
            _messages = result.Data.ToList();
            _metaData = result.MetaData;
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }

        _loadingMessages = false;
    }

    private void HandleRowClickEvent(TableRowClickEventArgs<MessageModel> tableRowClickEventArgs)
    {
        if (_selectedContainer == "sent")
        {
            NavManager.NavigateTo($"/member/{tableRowClickEventArgs.Item.RecipientUsername}/messages");
            return;
        }

        NavManager.NavigateTo($"/member/{tableRowClickEventArgs.Item.SenderUsername}/messages");
    }

    private async Task HandlePredicateChangeAsync(string predicate)
    {
        if (string.IsNullOrWhiteSpace(predicate))
        {
            return;
        }

        _selectedContainer = predicate.ToLowerInvariant();
        _messageFilter.Container = _selectedContainer switch
        {
            "sent" => "Sent",
            "unread" => "Unread",
            _ => "Inbox"
        };
        _messageFilter.PageNumber = 1;
        SetPageTitle(_selectedContainer);
        _messages = new();
        await LoadMessagesAsync();
    }

    private async Task HandlePageChangedAsync(int page)
    {
        _messageFilter.PageNumber = page;
        _messages = new();
        await LoadMessagesAsync();
    }

    private async Task HandleMessageDeleteAsync(int id)
    {
        ServiceResponseModel<string> result = await MessageService.DeleteMessageAsync(id);

        if (result.Success)
        {
            _showError = false;
            Snackbar.Add(result.Data, Severity.Success);
            await LoadMessagesAsync();
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }
    }

    private void SetPageTitle(string container)
    {
        _pageTitle = container switch
        {
            "sent" => "Messages - Sent",
            "unread" => "Messages - Unread",
            _ => "Messages - Inbox"
        };
    }
}
