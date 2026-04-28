namespace Client.Features.Members.Pages;

public partial class MemberList : IAsyncDisposable
{
    private const string PageSizeModeAuto = "auto";
    private List<MemberModel> _members = new();
    private MemberParameters _membersFilter = new();
    private PaginationModel _metaData = new();
    private bool _showError = false;
    private string _errorText = string.Empty;
    private bool _loadingMembers;
    private Breakpoint _breakpoint = Breakpoint.None;
    private string _selectedSort = "lastactive";
    private string _selectedPageSizeMode = PageSizeModeAuto;

    [Inject] IMemberStateService MemberStateService { get; set; }
    [Inject] IMemberService MemberService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _membersFilter.Gender = GetDefaultGenderFilter();
        if (_membersFilter.PageSize <= 0)
        {
            _membersFilter.PageSize = GetEffectivePageSize();
        }

        _membersFilter.OrderBy = "lastactive";
        await LoadMembersAsync();
    }

    private async Task HandleBreakpointChangedAsync(Breakpoint breakpoint)
    {
        if (breakpoint == _breakpoint)
        {
            return;
        }

        _breakpoint = breakpoint;

        int newPageSize = GetEffectivePageSize();

        if (_selectedPageSizeMode != PageSizeModeAuto || _membersFilter.PageSize == newPageSize)
        {
            return;
        }

        _membersFilter.PageSize = newPageSize;
        _membersFilter.PageNumber = 1;

        await LoadMembersAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task LoadMembersAsync()
    {
        _membersFilter.PageSize = GetEffectivePageSize();

        _loadingMembers = true;
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

        _loadingMembers = false;
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
            _membersFilter.PageNumber = 1;
            await LoadMembersAsync();
        }
    }

    private async Task HandleFilterResetAsync()
    {
        _membersFilter.PageNumber = 1;
        _membersFilter.MinAge = 18;
        _membersFilter.MaxAge = 45;
        _membersFilter.OrderBy = "lastactive";
        _selectedSort = "lastactive";
        _membersFilter.Gender = GetDefaultGenderFilter();
        _selectedPageSizeMode = PageSizeModeAuto;
        _membersFilter.PageSize = GetEffectivePageSize();

        await LoadMembersAsync();
    }

    private async Task HandleSortSubmitAsync(string sortValue)
    {
        _membersFilter.OrderBy = sortValue.ToLowerInvariant();
        _selectedSort = _membersFilter.OrderBy;
        _membersFilter.PageNumber = 1;
        await HandleFilterSubmitAsync();
    }

    private async Task HandlePageChangedAsync(int page)
    {
        _membersFilter.PageNumber = page;
        _members = new();
        await LoadMembersAsync();
    }

    private async Task HandlePageSizeModeChangedAsync(string pageSizeMode)
    {
        if (string.IsNullOrWhiteSpace(pageSizeMode))
        {
            return;
        }

        _selectedPageSizeMode = pageSizeMode;
        _membersFilter.PageNumber = 1;
        _membersFilter.PageSize = GetEffectivePageSize();
        await LoadMembersAsync();
    }

    private string GetDefaultGenderFilter()
    {
        return MemberStateService.Member.Gender.ToLowerInvariant().Equals("female")
            ? "male"
            : "female";
    }

    private int GetEffectivePageSize()
    {
        if (_selectedPageSizeMode == PageSizeModeAuto)
        {
            Breakpoint resolvedBreakpoint = _breakpoint == Breakpoint.None
                ? Breakpoint.Lg
                : _breakpoint;

            return GetAutoPageSize(resolvedBreakpoint);
        }

        bool parseResult = int.TryParse(_selectedPageSizeMode, out int fixedPageSize);
        if (parseResult && fixedPageSize > 0)
        {
            return fixedPageSize;
        }

        return GetAutoPageSize(Breakpoint.Lg);
    }

    private static int GetAutoPageSize(Breakpoint breakpoint)
    {
        return breakpoint switch
        {
            Breakpoint.Xxl => 6,
            Breakpoint.Xl => 6,
            Breakpoint.Lg => 6,
            Breakpoint.Md => 4,
            Breakpoint.Sm => 4,
            Breakpoint.Xs => 3,
            _ => 6
        };
    }

    private Variant GetSortVariant(string sortKey)
    {
        return _selectedSort == sortKey ? Variant.Filled : Variant.Outlined;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
