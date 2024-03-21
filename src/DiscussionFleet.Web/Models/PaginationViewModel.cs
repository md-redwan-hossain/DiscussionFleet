namespace DiscussionFleet.Web.Models;

public class PaginationViewModel
{
    public uint TotalData { get; set; }
    public byte DataPerPage { get; set; } = 15;
    public uint PageNeeded => (uint)Math.Ceiling((double)TotalData / DataPerPage);
    public uint CurrentPage { get; set; } = 1;
    public byte NextPage { get; set; }
    public byte PreviousPage { get; set; }
}