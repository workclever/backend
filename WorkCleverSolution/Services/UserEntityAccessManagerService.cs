using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Data;

namespace WorkCleverSolution.Services;

public interface IUserEntityAccessManagerService
{
    Task CreateUserEntityAccess(int userId, string permission, int entityId, string entityClass);
    Task DeleteUserEntityAccess(int userId, string permission, int entityId, string entityClass);
    Task DeleteAllEntityAccessByEntityId(int entityId, string entityClass);
    Task<List<UserEntityAccess>> ListMyAccessedEntities(int userId);
    Task<List<UserEntityAccess>> ListAccessedUsersByEntityId(int entityId, string entityClass);
    Task<bool> HasUserAccessOnEntity(int userId, string permission, int entityId, string entityClass);
}

public class UserEntityAccessManagerService : IUserEntityAccessManagerService
{
    private readonly IRepository<UserEntityAccess> _accessRepository;

    public UserEntityAccessManagerService(ApplicationDbContext dbContext)
    {
        _accessRepository = new Repository<UserEntityAccess>(dbContext);
    }

    public async Task CreateUserEntityAccess(int userId, string permission, int entityId, string entityClass)
    {
        var hasAccess = await HasUserAccessOnEntity(userId, permission, entityId, entityClass);
        if (hasAccess)
        {
            return;
        }

        var access = new UserEntityAccess
        {
            UserId = userId,
            Permission = permission,
            EntityId = entityId,
            EntityClass = entityClass
        };
        await _accessRepository.Create(access);
    }

    public async Task DeleteUserEntityAccess(int userId, string permission, int entityId, string entityClass)
    {
        var entity = await _accessRepository
            .Where(r => r.Permission == permission && r.UserId == userId &&
                        r.EntityId == entityId && r.EntityClass == entityClass)
            .SingleOrDefaultAsync();

        if (entity != null)
        {
            await _accessRepository.Delete(entity);
        }
    }

    public async Task DeleteAllEntityAccessByEntityId(int entityId, string entityClass)
    {
        var entity = await _accessRepository
            .Where(r => r.EntityId == entityId && r.EntityClass == entityClass)
            .SingleOrDefaultAsync();

        if (entity != null)
        {
            await _accessRepository.Delete(entity);
        }
    }

    public async Task<List<UserEntityAccess>> ListMyAccessedEntities(int userId)
    {
        var entities = await _accessRepository
            .Where(r => r.UserId == userId)
            .ToListAsync();

        return entities;
    }

    public async Task<List<UserEntityAccess>> ListAccessedUsersByEntityId(int entityId, string entityClass)
    {
        var entities = await _accessRepository
            .Where(r => r.EntityId == entityId && r.EntityClass == entityClass)
            .ToListAsync();

        return entities;
    }

    public async Task<bool> HasUserAccessOnEntity(int userId, string permission, int entityId, string entityClass)
    {
        var entity = await _accessRepository
            .Where(r => r.Permission == permission && r.UserId == userId &&
                        r.EntityId == entityId && r.EntityClass == entityClass)
            .SingleOrDefaultAsync();
        return entity != null;
    }
}