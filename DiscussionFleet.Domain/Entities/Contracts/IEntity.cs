namespace DiscussionFleet.Domain.Entities.Contracts;

public interface IEntity<TKey> where TKey : IEquatable<TKey>
{
    TKey Id { get; set; }
}