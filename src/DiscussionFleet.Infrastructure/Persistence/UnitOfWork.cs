using System.Data.Common;
using DiscussionFleet.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DiscussionFleet.Infrastructure.Persistence;

public abstract class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _appDbContext;

    protected UnitOfWork(DbContext appDbContext) => _appDbContext = appDbContext;

    public virtual void Dispose() => _appDbContext.Dispose();

    public virtual ValueTask DisposeAsync() => _appDbContext.DisposeAsync();

    public virtual void Save() => _appDbContext.SaveChanges();

    public virtual async Task SaveAsync() => await _appDbContext.SaveChangesAsync();

    public async Task<DbTransaction> BeginTransactionAsync()
    {
        var trx = await _appDbContext.Database.BeginTransactionAsync().ConfigureAwait(false);
        return trx.GetDbTransaction();
    }
}