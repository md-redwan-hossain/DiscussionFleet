namespace DiscussionFleet.Web.Models;

public class PaginationViewModel
{
    public uint TotalData { get; set; }
    public int StartPage { get; set; }
    public int EndPage { get; set; }
    public int DataPerPage { get; set; }
    public int CurrentPage { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
}