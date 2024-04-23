namespace DiscussionFleet.Domain.Common;

public abstract class Entity<TKey> : Timestamp, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public required TKey Id { get; set; }
}