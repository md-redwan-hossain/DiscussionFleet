namespace DiscussionFleet.Domain.Entities.Abstracts;

public abstract class Timestamp
{
    private bool _createdAtAssigned;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public void SetCreatedAtUtc(DateTime dateTime)
    {
        if (_createdAtAssigned || CreatedAtUtc != default) return;
        CreatedAtUtc = dateTime;
        _createdAtAssigned = true;
    }
}