namespace DiscussionFleet.Domain.Utils;

public record PagedData<T>(ICollection<T> Payload, int TotalCount);