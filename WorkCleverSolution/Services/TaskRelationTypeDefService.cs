using WorkCleverSolution.Data;
using WorkCleverSolution.Dto.Global;

namespace WorkCleverSolution.Services;

public interface ITaskRelationTypeDefService
{
    Task CreateTaskRelationTypeDef(CreateTaskRelationTypeDefInput input);
    Task UpdateTaskRelationTypeDef(UpdateTaskRelationTypeDefInput input);
    Task<List<TaskRelationTypeDef>> ListTaskRelationTypeDefs();
    Task DeleteTaskRelationTypeDef(int taskRelationTypeDefId);
    Task<TaskRelationTypeDef> GetById(int taskRelationTypeDefId);
}

public class TaskRelationTypeDefService : ITaskRelationTypeDefService
{
    private readonly IRepository<TaskRelationTypeDef> _taskRelationTypeDefRepository;

    public TaskRelationTypeDefService(ApplicationDbContext dbContext)
    {
        _taskRelationTypeDefRepository = new Repository<TaskRelationTypeDef>(dbContext);
    }


    public async Task CreateTaskRelationTypeDef(CreateTaskRelationTypeDefInput input)
    {
        var item = new TaskRelationTypeDef
        {
            Type = input.Type,
            InwardOperationName = input.InwardOperationName,
            OutwardOperationName = input.OutwardOperationName
        };
        await _taskRelationTypeDefRepository.Create(item);
    }

    public async Task UpdateTaskRelationTypeDef(UpdateTaskRelationTypeDefInput input)
    {
        var item = await GetById(input.Id);
        if (item == null)
        {
            return;
        }

        item.Type = input.Type;
        item.InwardOperationName = input.InwardOperationName;
        item.OutwardOperationName = input.OutwardOperationName;
        await _taskRelationTypeDefRepository.Update(item);
    }

    public async Task<List<TaskRelationTypeDef>> ListTaskRelationTypeDefs()
    {
        return await _taskRelationTypeDefRepository.GetAll();
    }

    public async Task DeleteTaskRelationTypeDef(int taskRelationTypeDefId)
    {
        var item = await GetById(taskRelationTypeDefId);
        if (item == null)
        {
            return;
        }

        await _taskRelationTypeDefRepository.Delete(item);
    }

    public async Task<TaskRelationTypeDef> GetById(int taskRelationTypeDefId)
    {
        return await _taskRelationTypeDefRepository.GetById(taskRelationTypeDefId);
    }
}