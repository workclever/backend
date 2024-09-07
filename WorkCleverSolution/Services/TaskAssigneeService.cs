using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Data;

namespace WorkCleverSolution.Services;

public interface ITaskAssigneeService
{
    Task SetTaskAssignees(int taskId, List<int> userIds);
    
    Task<List<int>> GetTaskAssignees(int taskId);
}

public class TaskAssigneeService : ITaskAssigneeService
{
    private readonly IRepository<TaskAssignee> _repository;
    private readonly IUserNotificationService _userNotificationService;


    public TaskAssigneeService(ApplicationDbContext dbContext,
        IUserNotificationService userNotificationService)
    {
        _userNotificationService = userNotificationService;
        _repository = new Repository<TaskAssignee>(dbContext);
    }

    public async Task SetTaskAssignees(int taskId, List<int> userIds)
    {
        var taskAssignees = await _repository.Where(r => r.TaskId == taskId).ToListAsync();
        var taskAssigneeUserIds = taskAssignees.Select(r => r.UserId).ToList();
        foreach (var userId in userIds)
        {
            if (taskAssigneeUserIds.Contains(userId))
            {
                // Already assigned user, do nothing
            }
            else
            {
                // Newly assigned user
                await _repository.Create(new TaskAssignee
                {
                    TaskId = taskId,
                    UserId = userId
                });
            }
        }

        foreach (var alreadyAssignedUser in taskAssignees)
        {
            if (userIds.Contains(alreadyAssignedUser.UserId))
            {
                // Already assigned user, do nothing
            }
            else
            {
                // Unassigned user, remove it
                await _repository.Delete(alreadyAssignedUser);
            }
        }
    }

    public async Task<List<int>> GetTaskAssignees(int taskId)
    {
        var taskAssignees = await _repository.Where(r => r.TaskId == taskId).ToListAsync();
        var taskAssigneeUserIds = taskAssignees.Select(r => r.UserId).ToList();
        return taskAssigneeUserIds;
    }
}