using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Data;
using WorkCleverSolution.Dto.Project.Task;

namespace WorkCleverSolution.Services;

public interface ITaskChangeLogService
{
    Task CreateChangeLog(int userId, TaskItem oldTaskItem, string property, string oldValue,
        string newValue);
    Task<List<TaskChangeLogOutput>> ListTaskChangeLog(int taskId);
}

public class TaskChangeLogService: ITaskChangeLogService
{
    private readonly ApplicationDbContext _dbContext;
    
    public TaskChangeLogService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task CreateChangeLog(int userId, TaskItem oldTaskItem, string property, string oldValue,
        string newValue)
    {
        var changeLog = new TaskChangeLog
        {
            ProjectId = oldTaskItem.ProjectId,
            BoardId = oldTaskItem.BoardId,
            TaskId = oldTaskItem.Id,
            UserId = userId,
            Property = property,
            OldValue = oldValue,
            NewValue = newValue
        };

        await _dbContext.TaskChangeLogs.AddAsync(changeLog);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<List<TaskChangeLogOutput>> ListTaskChangeLog(int taskId)
    {
        return await _dbContext
            .TaskChangeLogs
            .Where(r => r.TaskId == taskId)
            .Select(r => new TaskChangeLogOutput
            {
                Id = r.Id,
                Property = r.Property,
                OldValue = r.OldValue,
                NewValue = r.NewValue,
                DateCreated = r.DateCreated,
                UserId = r.UserId
            })
            .OrderBy(r => r.Id)
            .ToListAsync();
    }
}