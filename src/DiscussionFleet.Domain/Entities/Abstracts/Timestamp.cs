namespace DiscussionFleet.Domain.Entities.Abstracts;

public abstract class Timestamp
{
    private bool _createdAtAssigned;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; set; }

    public void SetCreatedAt(DateTime dateTime)
    {
        if (_createdAtAssigned || CreatedAt != default) return;
        CreatedAt = dateTime;
        _createdAtAssigned = true;
    }
}