using System.Data.Common;
using DiscussionFleet.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DiscussionFleet.Infrastructure.Persistence;

public abstract class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;

    protected UnitOfWork(DbContext dbContext) => _dbContext = dbContext;

    public virtual void Dispose() => _dbContext.Dispose();

    public virtual ValueTask DisposeAsync() => _dbContext.DisposeAsync();

    public virtual void Save() => _dbContext.SaveChanges();

    public virtual async Task SaveAsync() => await _dbContext.SaveChangesAsync();

    public DbTransaction BeginTransaction()
    {
        return _dbContext.Database.BeginTransaction().GetDbTransaction();
    }
}