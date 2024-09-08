using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Data;

namespace WorkCleverSolution.Services;

public interface ITaskAssigneeService
{
    Task SetTaskAssignees(int actorUserId, TaskItem task, List<int> userIds);

    Task<List<int>> GetTaskAssignees(int taskId);
    Task<List<int>> GetTaskAssigneesWithAllStakeholders(int taskId);
}

public class TaskAssigneeService : ITaskAssigneeService
{
    private readonly IRepository<TaskAssignee> _repository;
    private readonly IRepository<TaskItem> _taskRepository;
    private readonly IUserNotificationService _userNotificationService;
    private readonly ITaskChangeLogService _taskChangeLogService;


    public TaskAssigneeService(ApplicationDbContext dbContext,
        IUserNotificationService userNotificationService,
        ITaskChangeLogService taskChangeLogService)
    {
        _userNotificationService = userNotificationService;
        _taskChangeLogService = taskChangeLogService;
        _taskRepository = new Repository<TaskItem>(dbContext);;
        _repository = new Repository<TaskAssignee>(dbContext);
    }

    public async Task SetTaskAssignees(int actorUserId, TaskItem task, List<int> userIds)
    {
        var taskAssignees = await _repository.Where(r => r.TaskId == task.Id).ToListAsync();
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
                    TaskId = task.Id,
                    UserId = userId
                });
                if (userId != actorUserId)
                {
                    const string type = "TASK_ASSIGNED";
                    const string content = "Task is assigned to you.";
                    var byUserId = actorUserId;
                    var toUserId = userId;
                    await _userNotificationService.CreateNotification(byUserId, toUserId, type, content, task.Id);
                    await _taskChangeLogService.CreateChangeLog(
                        actorUserId,
                        task,
                        "TASK_ASSIGNED",
                        "",
                        toUserId.ToString());
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
                    await _userNotificationService.CreateNotification(byUserId, toUserId, type, content, task.Id);
                    await _taskChangeLogService.CreateChangeLog(
                        actorUserId,
                        task,
                        "TASK_UNASSIGNED",
                        "",
                        toUserId.ToString());
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

    public async Task<List<int>> GetTaskAssigneesWithAllStakeholders(int taskId)
    {
        var taskAssignees = await _repository.Where(r => r.TaskId == taskId).ToListAsync();
        var taskAssigneeUserIds = taskAssignees.Select(r => r.UserId).ToList();
        var task = await _taskRepository.GetById(taskId);

        if (task?.ReporterUserId == null)
        {
            return taskAssigneeUserIds;
        }

        taskAssigneeUserIds.Add((int)task.ReporterUserId);
        return taskAssigneeUserIds;
    }
}