namespace Client.Components;

public partial class MemberMessage
{
    [Inject] IMessageService MessageService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Parameter] public string Username { get; set; }

    private List<MessageModel> _messages = new();
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

}
