namespace Client.Features.Likes.Pages;

public partial class Lists
{
    private List<MemberModel> _members = new();
    private LikesParameters _likesFilter = new();
    private PaginationModel _metaData = new();
    private string _pageTitle = "Likes";
    private string _selectedPredicate = "likedby";
    private bool _loadingLikes;
    private bool _showError = false;
    private string _errorText = string.Empty;

    [Inject] ILikesService LikesService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(_likesFilter.Predicate))
        {
            _likesFilter.Predicate = "likedby";
        }

        _selectedPredicate = _likesFilter.Predicate.ToLowerInvariant();
        SetPageTitle(_selectedPredicate);

        if (_likesFilter.PageSize <= 0)
        {
            _likesFilter.PageSize = 8;
        }

        await LoadLikesAsync();
    }

    private async Task LoadLikesAsync()
    {
        _loadingLikes = true;
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

        _loadingLikes = false;
    }

    private async Task HandlePredicateChangeAsync(string predicate)
    {
        if (string.IsNullOrWhiteSpace(predicate))
        {
            return;
        }

        _selectedPredicate = predicate.ToLowerInvariant();
        _likesFilter.Predicate = _selectedPredicate;
        _likesFilter.PageNumber = 1;
        SetPageTitle(_selectedPredicate);
        _members = new();
        await LoadLikesAsync();
    }

    private async Task HandlePageChangedAsync(int page)
    {
        _likesFilter.PageNumber = page;
        _members = new();
        await LoadLikesAsync();
    }

    private void SetPageTitle(string predicate)
    {
        _pageTitle = predicate == "likedby"
            ? "Members who like me"
            : "Members who I like";
    }
}
