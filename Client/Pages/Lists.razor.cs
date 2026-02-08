namespace Client.Pages;

public partial class Lists
{
    private List<MemberModel> _members = new();
    private LikesParameters _likesFilter = new();
    private PaginationModel _metaData;
    private string _pageTitle;
    private bool _showError = false;
    private string _errorText;
    

    [Inject] ILikesService LikesService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

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

        if (_likesFilter.PageSize <= 0)
        {
            _likesFilter.PageSize = 8;
        }

        await LoadLikesAsync();
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
