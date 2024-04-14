using DiscussionFleet.Domain.Entities.QuestionAggregate.Utils;
using DiscussionFleet.Domain.Utils;

namespace DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;

public record QuestionFilterRequest(
    QuestionSortCriteria SortBy,
    QuestionFilterCriteria FilterBy,
    DataSortOrder SortOrder,
    int Page,
    int Limit,
    ICollection<Guid> Tags);