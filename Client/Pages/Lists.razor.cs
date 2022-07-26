namespace Client.Pages;

public partial class Lists
{
    private List<MemberModel> _members = new();
    private PaginationModel _metaData;
    private string _pageTitle;
    private bool _showError = false;
    private string _errorText;

    [Inject] ILikesService LikesService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(LikesService.LikesFilter.Predicate))
        {
            LikesService.LikesFilter.Predicate = "LikedBy";
        }

        if (LikesService.LikesFilter.Predicate == "LikedBy")
        {
            _pageTitle = "Members who like me";
        }
        else
        {
            _pageTitle = "Members who I like";
        }

        await LoadLikesAsync();
    }

    private async Task LoadLikesAsync()
    {
        PaginationResponseModel<IEnumerable<MemberModel>> result = await LikesService.GetLikesAsync(LikesService.LikesFilter);

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
            LikesService.LikesFilter.Predicate = "likedby";
            _pageTitle = "Members who like me";
        }
        else
        {
            LikesService.LikesFilter.Predicate = "liked";
            _pageTitle = "Members who I like";
        }

        _members = null;
        await LoadLikesAsync();
    }

    private async Task HandlePageChangedAsync(int page)
    {
        LikesService.LikesFilter.PageNumber = page;
        _members = null;
        await LoadLikesAsync();
    }
}
