namespace DiscussionFleet.Application.Common.Utils;

public class Paginator
{
    public Paginator(int totalItems, int currentPage = 1, int dataPerPage = 15)
    {
        if (dataPerPage <= 0) dataPerPage = 15;

        const int maxPages = 5;
        // calculate total pages
        var totalPages = (int)Math.Ceiling(totalItems / (decimal)dataPerPage);


        if (currentPage < 1)
        {
            currentPage = 1;
        }
        else if (currentPage > totalPages)
        {
            currentPage = totalPages;
        }

        int startPage, endPage;
        if (totalPages <= maxPages)
        {
            // total pages less than max so show all pages
            startPage = 1;
            endPage = totalPages;
        }
        else
        {
            // total pages more than max so calculate start and end pages
            var maxPagesBeforeCurrentPage = (int)Math.Floor(maxPages / (decimal)2);
            var maxPagesAfterCurrentPage = (int)Math.Ceiling(maxPages / (decimal)2) - 1;
            if (currentPage <= maxPagesBeforeCurrentPage)
            {
                // current page near the start
                startPage = 1;
                endPage = maxPages;
            }
            else if (currentPage + maxPagesAfterCurrentPage >= totalPages)
            {
                // current page near the end
                startPage = totalPages - maxPages + 1;
                endPage = totalPages;
            }
            else
            {
                // current page somewhere in the middle
                startPage = currentPage - maxPagesBeforeCurrentPage;
                endPage = currentPage + maxPagesAfterCurrentPage;
            }
        }

        // calculate start and end item indexes
        var startIndex = (currentPage - 1) * dataPerPage;
        var endIndex = Math.Min(startIndex + dataPerPage - 1, totalItems - 1);

        // create an array of pages that can be looped over
        var pages = Enumerable.Range(startPage, (endPage + 1) - startPage);

        // update object instance with all pager properties required by the view
        TotalItems = totalItems;
        CurrentPage = currentPage;
        DataPerPage = dataPerPage;
        TotalPages = totalPages;
        StartPage = startPage;
        EndPage = endPage;
        StartIndex = startIndex;
        EndIndex = endIndex;
        Pages = pages;
    }

    public int TotalItems { get; }
    public int CurrentPage { get; }
    public int DataPerPage { get; }
    public int TotalPages { get; }
    public int StartPage { get; }
    public int EndPage { get; }
    public int StartIndex { get; }
    public int EndIndex { get; }
    public IEnumerable<int> Pages { get; }
}