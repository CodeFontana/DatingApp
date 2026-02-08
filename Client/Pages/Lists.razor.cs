namespace Client.Pages;

public partial class Lists
{
    private List<MemberModel> _members = new();
    private LikesParameters _likesFilter = new();
    private PaginationModel _metaData;
    private string _pageTitle;
    private bool _showError = false;
    private string _errorText;
    private Guid _subscriptionId;

    [Inject] ILikesService LikesService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] IBreakpointService BreakpointListener { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(_likesFilter.Predicate))
        {
            _likesFilter.Predicate = "LikedBy";
        }

        if (_likesFilter.Predicate == "LikedBy")
        {
            _pageTitle = "Members who like me";
        }
        else
        {
            _pageTitle = "Members who I like";
        }

        await LoadLikesAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            BreakpointServiceSubscribeResult subscriptionResult = await BreakpointListener.Subscribe(async (breakpoint) =>
            {
                Console.WriteLine($"Current Breakpoint: {breakpoint}");

                if (breakpoint == Breakpoint.Xxl)
                {
                    _likesFilter.PageSize = 12;
                }
                else if (breakpoint == Breakpoint.Xl)
                {
                    _likesFilter.PageSize = 12;
                }
                else if (breakpoint == Breakpoint.Lg)
                {
                    _likesFilter.PageSize = 8;
                }
                else if (breakpoint == Breakpoint.Md)
                {
                    _likesFilter.PageSize = 8;
                }
                else if (breakpoint == Breakpoint.Sm)
                {
                    _likesFilter.PageSize = 6;
                }
                else if (breakpoint == Breakpoint.Xs)
                {
                    _likesFilter.PageSize = 10;
                }

                await LoadLikesAsync();
                await InvokeAsync(StateHasChanged);
            }, new ResizeOptions
            {
                ReportRate = 250,
                NotifyOnBreakpointOnly = true,
            });

            _subscriptionId = subscriptionResult.SubscriptionId;
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadLikesAsync()
    {
        PaginationResponseModel<IEnumerable<MemberModel>> result = await LikesService.GetLikesAsync(_likesFilter);

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

    private async Task HandlePredicateChange(string predicate)
    {
        if (predicate.ToLower() == "likedby")
        {
            _likesFilter.Predicate = "likedby";
            _pageTitle = "Members who like me";
        }
        else
        {
            _likesFilter.Predicate = "liked";
            _pageTitle = "Members who I like";
        }

        _members = null;
        await LoadLikesAsync();
    }

    private async Task HandlePageChangedAsync(int page)
    {
        _likesFilter.PageNumber = page;
        _members = null;
        await LoadLikesAsync();
    }
}
