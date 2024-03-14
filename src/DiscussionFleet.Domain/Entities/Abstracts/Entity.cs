using DiscussionFleet.Domain.Entities.Contracts;
using DiscussionFleet.Domain.Entities.Helpers;

namespace DiscussionFleet.Domain.Entities.Abstracts;

public abstract class Entity<TKey> : Timestamp, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
}