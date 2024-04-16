using System.Data.Common;

namespace DiscussionFleet.Domain.Utils;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    void Save();
    Task SaveAsync();
    Task<DbTransaction> BeginTransactionAsync();
}