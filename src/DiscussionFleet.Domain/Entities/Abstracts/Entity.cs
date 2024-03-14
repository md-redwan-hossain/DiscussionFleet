using DiscussionFleet.Domain.Entities.Contracts;

namespace DiscussionFleet.Domain.Entities.Abstracts;

public abstract class Entity<TKey> : Timestamp, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
}