namespace DiscussionFleet.Web.Models.Others;

public class NavbarUserInfoViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool HasNewNotification { get; set; }
}