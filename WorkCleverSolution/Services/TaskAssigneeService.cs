using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Data;

namespace WorkCleverSolution.Services;

public interface ITaskAssigneeService
{
    Task SetTaskAssignees(int actorUserId, int taskId, List<int> userIds);

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

    public async Task SetTaskAssignees(int actorUserId, int taskId, List<int> userIds)
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
                if (userId != actorUserId)
                {
                    const string type = "TASK_ASSIGNED";
                    const string content = "Task is assigned to you.";
                    var byUserId = actorUserId;
                    var toUserId = userId;
                    await _userNotificationService.CreateNotification(byUserId, toUserId, type, content, taskId);
                }
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
                if (alreadyAssignedUser.UserId != actorUserId)
                {
                    const string type = "TASK_UNASSIGNED";
                    const string content = "Task is unassigned from you.";
                    var byUserId = actorUserId;
                    var toUserId = alreadyAssignedUser.UserId;
                    await _userNotificationService.CreateNotification(byUserId, toUserId, type, content, taskId);
                }
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