using WorkCleverSolution.Data;
using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Dto.Project.Task;
using WorkCleverSolution.Utils;
using System.Security.Claims;

namespace WorkCleverSolution.Services;

public interface ITaskService
{
    Task<TaskOutput> CreateTask(int userId, CreateTaskInput input);
    Task<List<TaskOutput>> ListBoardTasks(int boardId);
    Task<List<TaskOutput>> ListProjectTasks(int projectId);
    Task<List<TaskOutput>> SearchTasks(ClaimsPrincipal user, string text, int projectId);
    Task DeleteTask(int userId, int taskId);
    Task<TaskItem> GetByIdInternal(int taskId);
    Task<TaskOutput> GetById(int taskId);
    Task MoveTaskToColumn(int userId, MoveTaskInput input);
    Task UpdateTaskProperty(int userId, UpdateTaskPropertyInput input);
    Task UpdateTaskAssigneeUser(int userId, UpdateTaskAssigneeUserInput input);
    Task<List<TaskChangeLogOutput>> ListTaskChangeLog(int taskId);

    Task CreateSubtaskRelation(int parentTaskItemId, int taskId);
    Task UploadTaskAttachmentInput(int userId, int taskId, UploadAttachmentInput input);

    Task<List<TaskAttachment>> ListTaskAttachments(int taskId);
    Task UpdateTaskOrders(UpdateTaskOrdersInput input);
    Task SendTaskToTopOrBottom(SendTaskToTopOrBottomInput input);
}

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRepository<TaskItem> _taskRepository;
    private readonly IRepository<TaskAttachment> _taskAttachmentRepository;
    private readonly IFileUploadService _fileUploadService;
    private readonly IUserNotificationService _userNotificationService;
    private readonly IUserService _userService;

    public TaskService(ApplicationDbContext dbContext,
        IFileUploadService fileUploadService,
        IUserNotificationService userNotificationService,
        IUserService userService)
    {
        _fileUploadService = fileUploadService;
        _userNotificationService = userNotificationService;
        _dbContext = dbContext;
        _userService = userService;
        _taskRepository = new Repository<TaskItem>(dbContext);
        _taskAttachmentRepository = new Repository<TaskAttachment>(dbContext);
    }

    private static TaskOutput MapTaskToOutput(TaskItem taskItem)
    {
        return new TaskOutput
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            ProjectId = taskItem.ProjectId,
            BoardId = taskItem.BoardId,
            ColumnId = taskItem.ColumnId,
            AssigneeUserId = taskItem.AssigneeUserId,
            ReporterUserId = taskItem.ReporterUserId,
            ParentTaskItemId = taskItem.ParentTaskItemId,
            Order = taskItem.Order,
            Slug = $"{taskItem.Project.Slug}-{taskItem.Id}"
        };
    }

    public async Task<TaskOutput> CreateTask(int userId, CreateTaskInput input)
    {
        var lastOrder = await _taskRepository
            .Where(r => r.ColumnId == input.ColumnId)
            .OrderByDescending(r => r.Order)
            .Select(r => r.Order)
            .FirstOrDefaultAsync();

        var task = new TaskItem
        {
            Title = input.Title,
            Description = input.Description,
            ProjectId = input.ProjectId,
            BoardId = input.BoardId,
            ColumnId = input.ColumnId,
            ReporterUserId = userId,
            AssigneeUserId = input.AssigneeUserId,
            ParentTaskItemId = input.ParentTaskItemId,
            Order = lastOrder == 0 ? 1 : lastOrder + 1
        };

        await _taskRepository.Create(task);

        return MapTaskToOutput(task);
    }

    public async Task<List<TaskOutput>> ListBoardTasks(int boardId)
    {
        var tasks = await _taskRepository
            .Where(r => r.BoardId == boardId)
            .Include(r => r.Project)
            .Select(r => MapTaskToOutput(r))
            .ToListAsync();

        return tasks;
    }

    public async Task<List<TaskOutput>> ListProjectTasks(int projectId)
    {
        var tasks = await _taskRepository
            .Where(r => r.ProjectId == projectId)
            .Include(r => r.Project)
            .Select(r => MapTaskToOutput(r))
            .ToListAsync();

        return tasks;
    }

    public async Task<List<TaskOutput>> SearchTasks(ClaimsPrincipal user, string text, int projectId)
    {
        text = text.ToLower().Trim();
        var userProjectIds = (await _userService.ListUserProjects(user)).Select(r => r.Id);

        if (projectId == 0)
        {

            return await _taskRepository
           .Where(r => userProjectIds.Contains(r.ProjectId) &&
                        (r.Description.ToLower().Contains(text) ||
                        r.Title.ToLower().Contains(text) ||
                        text.Contains(r.Id.ToString()))
           )
           .Select(r => MapTaskToOutput(r))
           .ToListAsync();
        }

        return await _taskRepository
        // TODO find a better way to incorporate [ValidProjectId]
            .Where(r => r.ProjectId == projectId && userProjectIds.Contains(projectId) &&
                        (r.Description.ToLower().Contains(text) ||
                         r.Title.ToLower().Contains(text) ||
                         text.Contains(r.Id.ToString()))
            )
            .Select(r => MapTaskToOutput(r))
            .ToListAsync();
    }

    // TODO: delete attachments as well
    public async Task DeleteTask(int userId, int taskId)
    {
        var task = await GetByIdInternal(taskId);
        await _taskRepository.Delete(task);
        // Cleanup everything related to a task, including comments, attachments, changelogs etc
        var comments = await _dbContext
            .TaskComments.Where(r => r.TaskId == taskId)
            .ToListAsync();

        var relations = await _dbContext
            .TaskRelations.Where(r => r.TaskId == taskId || r.TargetTaskId == taskId)
            .ToListAsync();

        var parentTasks = await _dbContext
            .TaskItems.Where(r => r.ParentTaskItemId == taskId)
            .ToListAsync();

        var customFieldValues = await _dbContext
            .TaskCustomFieldValues.Where(r => r.TaskId == taskId)
            .ToListAsync();

        var changeLogs = await _dbContext
            .TaskChangeLogs.Where(r => r.TaskId == taskId)
            .ToListAsync();

        var notifications = await _dbContext
            .UserNotifications.Where(r => r.TaskId == taskId)
            .ToListAsync();

        _dbContext.TaskComments.RemoveRange(comments);
        _dbContext.TaskRelations.RemoveRange(relations);
        _dbContext.TaskItems.RemoveRange(parentTasks);
        _dbContext.TaskCustomFieldValues.RemoveRange(customFieldValues);
        _dbContext.TaskChangeLogs.RemoveRange(changeLogs);
        _dbContext.UserNotifications.RemoveRange(notifications);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<TaskItem> GetByIdInternal(int taskId)
    {
        return await _taskRepository.GetById(taskId);
    }

    public async Task<TaskOutput> GetById(int taskId)
    {
        var task = await GetByIdInternal(taskId);
        return MapTaskToOutput(task);
    }

    public async Task MoveTaskToColumn(int userId, MoveTaskInput input)
    {
        await UpdateTaskProperty(userId, new UpdateTaskPropertyInput
        {
            TaskId = input.TaskId,
            Property = "BoardId",
            Value = input.TargetBoardId.ToString()
        });
        await UpdateTaskProperty(userId, new UpdateTaskPropertyInput
        {
            TaskId = input.TaskId,
            Property = "ColumnId",
            Value = input.TargetColumnId.ToString()
        });
    }

    private async Task CreateChangeLog(int userId, TaskItem oldTaskItem, string property, string oldValue,
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

    public async Task UpdateTaskProperty(int userId, UpdateTaskPropertyInput input)
    {
        var task = await GetByIdInternal(input.TaskId);
        var oldValue = ReflectionUtils.GetObjectPropertyValue(task, input.Property);
        var newValue = input.Value;

        // If old and new value are same, we don't need to update the task and create a changelog
        if (oldValue == newValue)
        {
            return;
        }

        ReflectionUtils.SetObjectProperty(task, input.Property, input.Value);
        await _taskRepository.Update(task);

        await CreateChangeLog(userId, task, input.Property, oldValue, newValue);
    }

    public async Task UpdateTaskAssigneeUser(int userId, UpdateTaskAssigneeUserInput input)
    {
        await UpdateTaskProperty(userId, new UpdateTaskPropertyInput
        {
            TaskId = input.TaskId,
            Property = "AssigneeUserId",
            Value = input.UserId.ToString()
        });
        // Operating user someone else, notify new assignee user
        if (userId != input.UserId)
        {
            const string type = "TASK_ASSIGNED";
            const string content = "Task is assigned to you.";
            var byUserId = userId;
            var toUserId = input.UserId;
            await _userNotificationService.CreateNotification(byUserId, toUserId, type, content, input.TaskId);
        }
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

    public async Task CreateSubtaskRelation(int parentTaskItemId, int taskId)
    {
        if (parentTaskItemId == taskId)
        {
            throw new ApplicationException("SUBTASK_CANT_HAVE_ITSELF_AS_PARENT");
        }

        var subtask = await GetByIdInternal(taskId);
        if (subtask.ParentTaskItemId != null)
        {
            throw new ApplicationException("SUBTASK_ALREADY_HAS_A_PARENT_TASK");
        }

        if (parentTaskItemId == subtask.ParentTaskItemId)
        {
            throw new ApplicationException("SUBTASK_ALREADY_HAS_SAME_PARENT_TASK");
        }

        subtask.ParentTaskItemId = parentTaskItemId;
        await _taskRepository.Update(subtask);
    }

    public async Task UploadTaskAttachmentInput(int userId, int taskId, UploadAttachmentInput input)
    {
        var task = await GetByIdInternal(taskId);
        var fileDiskName = Guid.NewGuid().ToString();
        var fileVisibleName = Path.GetFileName(input.file.FileName);
        var accessUrl = await _fileUploadService.UploadFile(
            fileDiskName,
            new[] { "projects", task.ProjectId.ToString() },
            input.file
        );
        await _taskAttachmentRepository.Create(new TaskAttachment
        {
            TaskId = taskId,
            Name = fileVisibleName,
            AttachmentUrl = accessUrl
        });

        if (task.AssigneeUserId == null)
        {
            return;
        }

        // Operating user someone else, notify task assignee user
        if (userId != task.AssigneeUserId)
        {
            const string type = "TASK_ATTACHMENT_UPLOADED";
            const string content = "A file is attached.";
            var byUserId = userId;
            var toUserId = task.AssigneeUserId.Value;
            await _userNotificationService.CreateNotification(byUserId, toUserId, type, content, taskId);
        }
    }

    public async Task<List<TaskAttachment>> ListTaskAttachments(int taskId)
    {
        return await _taskAttachmentRepository.Where(r => r.TaskId == taskId).ToListAsync();
    }

    // TODO: Improve later as this implementation is prototyped quickly and not performant
    // At least make a batch update, instead of getting them one by one and updating one by one
    public async Task UpdateTaskOrders(UpdateTaskOrdersInput input)
    {
        foreach (var entry in input.GroupedTasks)
        {
            var columnId = entry.Key;
            var taskIds = entry.Value;

            var index = 0;
            foreach (var taskId in taskIds)
            {
                var task = await _taskRepository.GetById(taskId);
                task.ColumnId = columnId;
                task.Order = index;
                await _taskRepository.Update(task);
                index++;
            }
        }
    }

    public async Task SendTaskToTopOrBottom(SendTaskToTopOrBottomInput input)
    {
        var task = await GetByIdInternal(input.TaskId);
        // Get the tasks in column for reordering, exclude the given task id,
        // Since we are going to push it to top or bottom in the next step
        var tasksInColumn = await _taskRepository
            .Where(r => r.ColumnId == task.ColumnId && r.Id != input.TaskId)
            // Preserve current order
            .OrderBy(r => r.Order)
            .ToListAsync();

        if (input.Location == 1)
        {
            tasksInColumn.Insert(0, task);
        }
        else
        {
            tasksInColumn.Add(task);
        }

        var index = 0;
        foreach (var taskInColumn in tasksInColumn)
        {
            taskInColumn.Order = index;
            await _taskRepository.Update(taskInColumn);
            index++;
        }
    }
}