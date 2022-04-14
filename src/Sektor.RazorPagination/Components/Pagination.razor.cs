using Microsoft.AspNetCore.Components;
using Sektor.RazorPagination.Models;

namespace Sektor.RazorPagination.Components;

public partial class Pagination
{
    #region Params

    [Parameter]
    public int ItemsCount { get; set; }

    [Parameter]
    public PagingOptions PagingOptions { get; set; } = default!;

    [Parameter]
    public EventCallback<int> OnPageSelected { get; set; }

    #endregion

    public bool Visible { get; set; } = false;

    private int _currentPage;
    private int _totalPages;

    private bool _hasPreviousPage => _currentPage > 1;
    private bool _hasNextPage => _currentPage < _totalPages;

    private List<PagingLink> _pageLinks;
    private PagingLink _firstLink;
    private PagingLink _prevLink;
    private PagingLink _nextLink;
    private PagingLink _lastLink;

    protected override void OnParametersSet()
    {
        CalcTotalPages();
        CreatePaginationLinks();
    }

    private void CreatePaginationLinks()
    {
        if (_totalPages > 1)
        {
            Visible = true;
            _pageLinks = new List<PagingLink>();
            _firstLink = new PagingLink(1, _hasPreviousPage, "<<");
            _prevLink = new PagingLink(_currentPage - 1, _hasPreviousPage, "<");
            _nextLink = new PagingLink(_currentPage + 1, _hasNextPage, ">");
            _lastLink = new PagingLink(_totalPages, _hasNextPage, ">>");

            for (var i = 1; i <= _totalPages; i++)
            {
                if (i >= _currentPage - PagingOptions.PageSpread && i <= _currentPage + PagingOptions.PageSpread)
                {
                    _pageLinks.Add(new PagingLink(i, true, i.ToString()) { Active = _currentPage == i });
                }
            }
        }
    }

    private void CalcTotalPages() => _totalPages = (int)Math.Ceiling(ItemsCount / (double)PagingOptions.ItemsOnPage);

    private async Task OnSelectedPage(PagingLink link)
    {
        if (link.Page == _currentPage || !link.Enabled)
            return;
        _currentPage = link.Page;
        await OnPageSelected.InvokeAsync(link.Page);
    }
}
