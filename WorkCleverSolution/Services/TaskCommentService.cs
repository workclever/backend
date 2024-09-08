using WorkCleverSolution.Data;
using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Dto.Project.Comment;

namespace WorkCleverSolution.Services;

public interface ITaskCommentService
{
    Task<TaskComment> GetById(int taskCommentId);
    Task<Dictionary<int, List<TaskCommentOutput>>> ListTaskCommentsByBoardId(int boardId);
    Task CreateTaskComment(int userId, CreateTaskCommentInput input);
    Task UpdateTaskComment(int userId, UpdateTaskCommentInput input);
    Task DeleteTaskComment(int userId, DeleteTaskCommentInput input);
}

public class TaskCommentService : ITaskCommentService
{
    private readonly IRepository<TaskItem> _taskRepository;
    private readonly IRepository<TaskComment> _taskCommentRepository;
    private readonly IUserNotificationService _userNotificationService;
    private readonly ITaskAssigneeService _taskAssigneeService;

    public TaskCommentService(
        ApplicationDbContext dbContext,
        IUserNotificationService userNotificationService,
        ITaskAssigneeService taskAssigneeService)
    {
        _userNotificationService = userNotificationService;
        _taskAssigneeService = taskAssigneeService;
        _taskCommentRepository = new Repository<TaskComment>(dbContext);
        _taskRepository = new Repository<TaskItem>(dbContext);
    }

    private static TaskCommentOutput MapTaskCommentToOutput(TaskComment r)
    {
        return new TaskCommentOutput
        {
            Id = r.Id,
            Content = r.Content,
            DateCreated = r.DateCreated,
            UserId = r.UserId,
            TaskId = r.TaskId
        };
    }

    public async Task<TaskComment> GetById(int taskCommentId)
    {
        return await _taskCommentRepository.GetById(taskCommentId);
    }

    public async Task<Dictionary<int, List<TaskCommentOutput>>> ListTaskCommentsByBoardId(int boardId)
    {
        var taskIds = await _taskRepository
            .Where(r => r.BoardId == boardId)
            .Select(r => r.Id)
            .ToListAsync();

        var allTaskComments = await _taskCommentRepository
            .Where(r => taskIds.Contains(r.TaskId))
            .ToListAsync();

        var dict = new Dictionary<int, List<TaskCommentOutput>>();

        foreach (var comment in allTaskComments)
        {
            if (dict.ContainsKey(comment.TaskId))
            {
                dict[comment.TaskId].Add(MapTaskCommentToOutput(comment));
            }
            else
            {
                dict[comment.TaskId] = new List<TaskCommentOutput>();
                dict[comment.TaskId].Add(MapTaskCommentToOutput(comment));
            }
        }

        return dict;
    }

    public async Task CreateTaskComment(int userId, CreateTaskCommentInput input)
    {
        var comment = new TaskComment
        {
            TaskId = input.TaskId,
            UserId = userId,
            Content = input.Content,
        };
        await _taskCommentRepository.Create(comment);

        // Operating user someone else, notify task assignee user
        foreach (var taskAssignee in await _taskAssigneeService.GetTaskAssigneesWithAllStakeholders(input.TaskId))
        {
            const string type = "TASK_COMMENTED";
            const string content = "A comment is made.";
            var byUserId = userId;
            var toUserId = taskAssignee;
            await _userNotificationService.CreateNotification(byUserId, toUserId, type, content, input.TaskId);
        }
    }

    public async Task UpdateTaskComment(int userId, UpdateTaskCommentInput input)
    {
        var comment = await _taskCommentRepository
            .Where(r => r.Id == input.CommentId && r.TaskId == input.TaskId && r.UserId == userId)
            .SingleOrDefaultAsync();

        comment.Content = input.Content;
        await _taskCommentRepository.Update(comment);
    }

    public async Task DeleteTaskComment(int userId, DeleteTaskCommentInput input)
    {
        var comment = await _taskCommentRepository
            .Where(r => r.Id == input.CommentId && r.TaskId == input.TaskId && r.UserId == userId)
            .SingleOrDefaultAsync();

        await _taskCommentRepository.Delete(comment);
    }
}