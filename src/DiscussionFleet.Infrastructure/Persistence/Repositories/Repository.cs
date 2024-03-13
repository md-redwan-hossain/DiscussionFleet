using System.Linq.Expressions;
using DiscussionFleet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public abstract class Repository<TEntity, TKey>
    : IRepositoryBase<TEntity, TKey>
    where TEntity : class
    where TKey : IEquatable<TKey>
{
    private readonly DbContext _dbContext;
    protected readonly DbSet<TEntity> EntityDbSet;

    protected Repository(DbContext context)
    {
        _dbContext = context;
        EntityDbSet = _dbContext.Set<TEntity>();
    }

    public virtual async Task CreateAsync(TEntity entity)
    {
        await EntityDbSet.AddAsync(entity);
    }

    public virtual async Task<TEntity?> GetOneAsync(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, object>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        var data = EntityDbSet.Where(filter);
        if (include is not null) data = data.Include(include);
        if (disableTracking) data.AsNoTracking();
        return await data.FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TResult?> GetOneAsync<TResult>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, object>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        var data = EntityDbSet.Where(filter);
        if (include is not null) data = data.Include(include);
        if (disableTracking) data.AsNoTracking();
        return await data.Select(selector).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<IList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, object>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        var data = EntityDbSet.AsQueryable();
        if (include is not null) data = data.Include(include);
        if (disableTracking) data.AsNoTracking();
        return await data.ToListAsync(cancellationToken);
    }

    public virtual Task UpdateAsync(TEntity entityToUpdate)
    {
        return Task.Run(() =>
        {
            EntityDbSet.Attach(entityToUpdate);
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        });
    }

    public virtual async Task<bool> RemoveAsync(TKey id)
    {
        var entity = await EntityDbSet.FindAsync(id);
        if (entity is null) return false;
        await RemoveAsync(entity);
        return true;
    }

    public virtual Task RemoveAsync(TEntity entityToDelete)
    {
        return Task.Run(() =>
        {
            if (_dbContext.Entry(entityToDelete).State is EntityState.Detached)
            {
                EntityDbSet.Attach(entityToDelete);
            }

            EntityDbSet.Remove(entityToDelete);
        });
    }

    public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        int count;

        if (filter is not null) count = await EntityDbSet.CountAsync(filter, cancellationToken);
        else count = await EntityDbSet.CountAsync(cancellationToken);

        return count;
    }
}