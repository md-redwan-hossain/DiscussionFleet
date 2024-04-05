using System.Data.Common;

namespace DiscussionFleet.Domain;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    void Save();
    Task SaveAsync();
    DbTransaction BeginTransaction();
}