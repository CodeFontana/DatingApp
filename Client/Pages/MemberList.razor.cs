namespace Client.Pages;

public partial class MemberList : IAsyncDisposable
{
    private List<MemberModel> _members = new();
    private MemberParameters _membersFilter = new();
    private PaginationModel _metaData;
    private bool _showError = false;
    private string _errorText;
    private Guid _subscriptionId;

    [Inject] IMemberStateService MemberStateService { get; set; }
    [Inject] IMemberService MemberService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] IBreakpointService BreakpointListener { get; set; }

    protected override void OnInitialized()
    {
        if (string.IsNullOrWhiteSpace(_membersFilter.Gender))
        {
            if (MemberStateService.AppUser.Gender.ToLower().Equals("female"))
            {
                _membersFilter.Gender = "male";
            }
            else
            {
                _membersFilter.Gender = "female";
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            BreakpointServiceSubscribeResult subscriptionResult = await BreakpointListener.Subscribe(async (breakpoint) =>
            {
                await ScalePageSize(breakpoint);
            }, new ResizeOptions
            {
                ReportRate = 250,
                NotifyOnBreakpointOnly = true,
            });


            Breakpoint start = subscriptionResult.Breakpoint;
            await ScalePageSize(start);
            _subscriptionId = subscriptionResult.SubscriptionId;
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task ScalePageSize(Breakpoint breakpoint)
    {
        Console.WriteLine($"Current Breakpoint: {breakpoint}");

        if (breakpoint == Breakpoint.Xxl)
        {
            _membersFilter.PageSize = 12;
        }
        else if (breakpoint == Breakpoint.Xl)
        {
            _membersFilter.PageSize = 12;
        }
        else if (breakpoint == Breakpoint.Lg)
        {
            _membersFilter.PageSize = 8;
        }
        else if (breakpoint == Breakpoint.Md)
        {
            _membersFilter.PageSize = 8;
        }
        else if (breakpoint == Breakpoint.Sm)
        {
            _membersFilter.PageSize = 6;
        }
        else if (breakpoint == Breakpoint.Xs)
        {
            _membersFilter.PageSize = 10;
        }

        await LoadMembersAsync();
        await InvokeAsync(StateHasChanged);
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

        if (MemberStateService.AppUser.Gender.ToLower().Equals("female"))
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

    public async ValueTask DisposeAsync() => await BreakpointListener.Unsubscribe(_subscriptionId);
}
