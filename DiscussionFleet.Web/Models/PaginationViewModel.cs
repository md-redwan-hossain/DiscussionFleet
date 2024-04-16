namespace DiscussionFleet.Web.Models;

public class PaginationViewModel
{
    public int TotalData { get; set; }
    public int StartPage { get; set; }
    public int EndPage { get; set; }
    public int DataPerPage { get; set; } = 15;
    public int CurrentPage { get; set; } = 1;
    public string AreaName { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
}