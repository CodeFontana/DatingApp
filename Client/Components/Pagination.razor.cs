using DataAccessLibrary.Pagination;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Components;

public partial class Pagination
{
    [Parameter]
    public PaginationModel MetaData { get; set; }

    [Parameter]
    public int Spread { get; set; }

    [Parameter]
    public EventCallback<int> PageChanged { get; set; }

    private List<PaginationLinkModel> links;

    protected override void OnParametersSet()
    {
        CreatePaginationLinks();
    }

    private void CreatePaginationLinks()
    {
        links = new List<PaginationLinkModel>();
        links.Add(new PaginationLinkModel(MetaData.CurrentPage - 1, MetaData.HasPrevious, "Previous"));

        for (int i = 1; i <= MetaData.TotalPages; i++)
        {
            if (i >= MetaData.CurrentPage - Spread && i <= MetaData.CurrentPage + Spread)
            {
                links.Add(new PaginationLinkModel(i, true, i.ToString()) { Active = MetaData.CurrentPage == i });
            }
        }

        links.Add(new PaginationLinkModel(MetaData.CurrentPage + 1, MetaData.HasNext, "Next"));
    }

    private async Task OnSelectedPageAsync(PaginationLinkModel link)
    {
        if (link.Page == MetaData.CurrentPage || link.Enabled == false)
        {
            return;
        }

        MetaData.CurrentPage = link.Page;
        await PageChanged.InvokeAsync(link.Page);
    }
}
