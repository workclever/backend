using WorkCleverSolution.Data;
using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Dto.Project.Task;

namespace WorkCleverSolution.Services;

public interface ITaskRelationService
{
    Task CreateTaskRelation(int userId, CreateTaskRelationInput input);
    Task UpdateTaskRelation(int userId, UpdateTaskRelationInput input);
    Task<List<TaskRelationOutput>> ListTaskRelations(int taskId);
    Task DeleteTaskRelation(int taskRelationId);
}

public class TaskRelationService : ITaskRelationService
{
    private readonly IRepository<TaskRelation> _taskRelationRepository;
    private readonly IRepository<TaskParentRelation> _taskParentRelationRepository;

    public TaskRelationService(ApplicationDbContext dbContext)
    {
        _taskRelationRepository = new Repository<TaskRelation>(dbContext);
        _taskParentRelationRepository = new Repository<TaskParentRelation>(dbContext);
    }

    public async Task CreateTaskRelation(int userId, CreateTaskRelationInput input)
    {
        var existingTaskBaseRelation = await _taskRelationRepository
            .Where(r => r.TaskId == input.BaseTaskId && r.TargetTaskId == input.TargetTaskId)
            .SingleOrDefaultAsync();

        var existingTaskTargetRelation = await _taskRelationRepository
            .Where(r => r.TaskId == input.TargetTaskId && r.TargetTaskId == input.BaseTaskId)
            .SingleOrDefaultAsync();

        if (existingTaskBaseRelation != null || existingTaskTargetRelation != null)
        {
            throw new ApplicationException("TASKS_ARE_ALREADY_RELATED");
        }

        if (input.BaseTaskId == input.TargetTaskId)
        {
            throw new ApplicationException("TASK_CANT_SELF_RELATE");
        }

        var taskParentRelation = new TaskParentRelation
        {
            RelationTypeDefId = input.RelationTypeDefId,
        };
        await _taskParentRelationRepository.Create(taskParentRelation);

        var taskBaseRelation = new TaskRelation
        {
            UserId = userId,
            TaskId = input.BaseTaskId,
            TargetTaskId = input.TargetTaskId,
            TaskParentRelationId = taskParentRelation.Id,
            RelationTypeDirection = "INWARD"
        };

        var taskTargetRelation = new TaskRelation
        {
            UserId = userId,
            TaskId = input.TargetTaskId,
            TargetTaskId = input.BaseTaskId,
            TaskParentRelationId = taskParentRelation.Id,
            RelationTypeDirection = "OUTWARD"
        };

        await _taskRelationRepository.Create(taskBaseRelation);
        await _taskRelationRepository.Create(taskTargetRelation);
    }

    public async Task UpdateTaskRelation(int userId, UpdateTaskRelationInput input)
    {
        var taskParentRelation = await _taskParentRelationRepository
            .Where(r => r.Id == input.TaskParentRelationId)
            .SingleOrDefaultAsync();

        taskParentRelation.RelationTypeDefId = input.RelationTypeDefId;
        await _taskParentRelationRepository.Update(taskParentRelation);
    }

    public async Task<List<TaskRelationOutput>> ListTaskRelations(int taskId)
    {
        var normalRelations = await _taskRelationRepository
            .Where(r => r.TaskId == taskId)
            .Include(r => r.TaskParentRelation)
            .Select(r => new TaskRelationOutput
            {
                Id = r.Id,
                RelationTypeDefId = r.TaskParentRelation.RelationTypeDefId,
                RelationTypeDirection = r.RelationTypeDirection,
                TaskId = r.TargetTaskId,
                TaskParentRelationId = r.TaskParentRelationId,
            })
            .ToListAsync();
        return normalRelations;
    }

    public async Task DeleteTaskRelation(int taskParentRelationId)
    {
        // Removing parent removes child relations from DB
        var taskParentRelation = await _taskParentRelationRepository
            .Where(r => r.Id == taskParentRelationId)
            .SingleOrDefaultAsync();

        if (taskParentRelation != null)
        {
            await _taskParentRelationRepository.Delete(taskParentRelation);
        }
    }
}