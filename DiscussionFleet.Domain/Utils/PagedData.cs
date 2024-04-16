namespace DiscussionFleet.Domain.Utils;

public record PagedData<T>(IEnumerable<T> Payload, int TotalCount);