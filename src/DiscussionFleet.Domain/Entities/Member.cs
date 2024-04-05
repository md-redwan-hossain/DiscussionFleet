using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class Member : Entity<Guid>
{
    public int ReputationCount { get; set; } = 1;
    public string DisplayName { get; set; }
    public string? FullName { get; set; }
    public string? Location { get; set; }
    public string? Bio { get; set; }
    public string? PersonalWebsiteUrl { get; set; }
    public string? TwitterHandle { get; set; }
    public string? GitHubHandle { get; set; }
    public Guid? ProfileImageId { get; set; }
    public ICollection<MemberBadge> MemberBadges { get; set; }
    public ICollection<SavedQuestion> SavedQuestions { get; set; }
    public ICollection<SavedAnswer> SavedAnswers { get; set; }
}