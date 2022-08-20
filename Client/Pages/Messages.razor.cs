namespace Client.Pages;

public partial class Messages
{
    private MudTable<MessageModel> _messageTable;
	private List<MessageModel> _messages = new();
    private MessageParameters _messageFilter = new();
    private PaginationModel _metaData;
    private string _pageTitle;
    private bool _loadingMessages = false;
    private bool _showError = false;
    private string _errorText;

    [Inject] IMessageService MessageService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] NavigationManager NavManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(_messageFilter.Container))
        {
            _pageTitle = "Messages - Inbox";
            _messageFilter.Container = "Inbox";
        }

        await LoadMessagesAsync();
    }

    private async Task LoadMessagesAsync()
    {
        _loadingMessages = true;
        PaginationResponseModel<IEnumerable<MessageModel>> result = await MessageService.GetMessagesForMemberAsync(_messageFilter);

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
        NavManager.NavigateTo($"/member/{tableRowClickEventArgs.Item.SenderUsername}/3");
    }

    private async Task HandlePredicateChange(string predicate)
    {
        if (predicate.ToLower() == "inbox")
        {
            _messageFilter.Container = "Inbox";
            _pageTitle = "Messages - Inbox";
        }
        else if (predicate.ToLower() == "sent")
        {
            _messageFilter.Container = "Sent";
            _pageTitle = "Messages - Sent";
        }
        else
        {
            _messageFilter.Container = "Unread";
            _pageTitle = "Messages - Unread";
        }

        _messages = null;
        await LoadMessagesAsync();
    }

    private async Task HandlePageChangedAsync(int page)
    {
        _messageFilter.PageNumber = page;
        _messages = null;
        await LoadMessagesAsync();
    }
}
