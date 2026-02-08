namespace Client.Pages;

public partial class MemberList : IAsyncDisposable
{
    private List<MemberModel> _members = new();
    private MemberParameters _membersFilter = new();
    private PaginationModel _metaData;
    private bool _showError = false;
    private string _errorText;


    [Inject] IMemberStateService MemberStateService { get; set; }
    [Inject] IMemberService MemberService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

    protected override void OnInitialized()
    {
        if (string.IsNullOrWhiteSpace(_membersFilter.Gender))
        {
            if (MemberStateService.Member.Gender.ToLower().Equals("female"))
            {
                _membersFilter.Gender = "male";
            }
            else
            {
                _membersFilter.Gender = "female";
            }
        }

        if (_membersFilter.PageSize <= 0)
        {
            _membersFilter.PageSize = 8;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadMembersAsync();
            StateHasChanged();
        }
    }

    private async Task LoadMembersAsync()
    {
        PaginationResponseModel<IEnumerable<MemberModel>> result = await MemberService.GetMembersAsync(_membersFilter);

        if (result.Success)
        {
            _showError = false;
            _members = result.Data.ToList();
            _metaData = result.MetaData;
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }
    }

    private async Task HandleFilterSubmitAsync()
    {
        if (_membersFilter.MinAge > _membersFilter.MaxAge)
        {
            _showError = true;
            _errorText = "Please validate age filters";
        }
        else
        {
            _showError = false;
            await LoadMembersAsync();
        }
    }

    private async Task HandleFilterResetAsync()
    {
        _membersFilter.PageNumber = 1;
        _membersFilter.MinAge = 18;
        _membersFilter.MaxAge = 45;
        _membersFilter.OrderBy = "LastActive";

        if (MemberStateService.Member.Gender.ToLower().Equals("female"))
        {
            _membersFilter.Gender = "male";
        }
        else
        {
            _membersFilter.Gender = "female";
        }

        await LoadMembersAsync();
    }

    private async Task HandleSortSubmitAsync(string sortValue)
    {
        _membersFilter.OrderBy = sortValue.ToLower();
        await HandleFilterSubmitAsync();
    }

    private async Task HandlePageChangedAsync(int page)
    {
        _membersFilter.PageNumber = page;
        _members = null;
        await LoadMembersAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
