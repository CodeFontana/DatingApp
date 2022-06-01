namespace DataAccessLibrary.Pagination;

public class PaginationLinkModel
{
    public PaginationLinkModel(int page, bool enabled, string text)
    {
        Page = page;
        Enabled = enabled;
        Text = text;
    }

    public string Text { get; set; }
    public int Page { get; set; }
    public bool Enabled { get; set; }
    public bool Active { get; set; }
}
