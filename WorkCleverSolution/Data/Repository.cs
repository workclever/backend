using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace WorkCleverSolution.Data;

public interface IRepository<T> where T : TimeAwareEntity
{
    Task<List<T>> GetAll();
    Task<T> GetById(int id);
    Task<T> GetByIdWithIncludes(int id, params Expression<Func<T, object>>[] includeProperties);
    IQueryable<T> Where(Expression<Func<T, bool>> predicate);
    Task Create(T entity);
    Task Update(T entity);
    Task UpdateRange(List<T> entity);
    Task Delete(T entity);
    Task DeleteRange(List<T> entities);
}

public class Repository<T> : IRepository<T> where T : TimeAwareEntity
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<T> _entities;

    public Repository(ApplicationDbContext dbContext)
    {
        this._dbContext = dbContext;
        _entities = dbContext.Set<T>();
    }

    public async Task<List<T>> GetAll()
    {
        return await _entities.ToListAsync();
    }

    public async Task<T> GetById(int id)
    {
        return await _entities.SingleOrDefaultAsync(r => r.Id == id);
    }
    
    public async Task<T> GetByIdWithIncludes(int id, params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _entities;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.SingleOrDefaultAsync(r => r.Id == id);
    }

    public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
    {
        return _entities.Where(predicate);
    }

    public async Task Create(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }

        await _entities.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }

        _entities.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateRange(List<T> entity)
    {
        _entities.UpdateRange(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }

        _entities.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteRange(List<T> entities)
    {
        if (entities == null)
        {
            throw new ArgumentNullException("entities");
        }

        _entities.RemoveRange(entities);
        await _dbContext.SaveChangesAsync();
    }
}