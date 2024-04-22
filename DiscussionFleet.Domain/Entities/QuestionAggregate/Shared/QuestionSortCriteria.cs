namespace DiscussionFleet.Domain.Entities.QuestionAggregate.Shared;

public enum QuestionSortCriteria : byte
{
    Newest = 1,
    RecentActivity,
    HighestScore
}