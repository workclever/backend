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
    private readonly ITaskAssigneeService _taskAssigneeService;
    private readonly ITaskChangeLogService _taskChangeLogService;

    public TaskService(ApplicationDbContext dbContext,
        IFileUploadService fileUploadService,
        IUserNotificationService userNotificationService,
        IUserService userService,
        ITaskAssigneeService taskAssigneeService,
        ITaskChangeLogService taskChangeLogService)
    {
        _fileUploadService = fileUploadService;
        _userNotificationService = userNotificationService;
        _dbContext = dbContext;
        _userService = userService;
        _taskAssigneeService = taskAssigneeService;
        _taskChangeLogService = taskChangeLogService;
        _taskRepository = new Repository<TaskItem>(dbContext);
        _taskAttachmentRepository = new Repository<TaskAttachment>(dbContext);
    }

    private static TaskOutput MapTaskToOutput(TaskItem taskItem, List<int> assigneeUserIds)
    {
        return new TaskOutput
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            ProjectId = taskItem.ProjectId,
            BoardId = taskItem.BoardId,
            ColumnId = taskItem.ColumnId,
            AssigneeUserIds = assigneeUserIds,
            ReporterUserId = taskItem.ReporterUserId,
            ParentTaskItemId = taskItem.ParentTaskItemId,
            Order = taskItem.Order,
            Slug = $"{taskItem.Project.Slug}-{taskItem.Id}"
        };
    }

    private async Task<List<TaskOutput>> ProcessTasksWithAssignees(List<TaskItem> tasks)
    {
        var taskOutputs = new List<TaskOutput>();

        foreach (var task in tasks)
        {
            var assigneeUserIds = await _taskAssigneeService.GetTaskAssignees(task.Id);
            taskOutputs.Add(MapTaskToOutput(task, assigneeUserIds));
        }

        return taskOutputs;
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
            ParentTaskItemId = input.ParentTaskItemId,
            Order = lastOrder + 100000
        };

        await _taskRepository.Create(task);

        return MapTaskToOutput(task, await _taskAssigneeService.GetTaskAssignees(task.Id));
    }

    public async Task<List<TaskOutput>> ListBoardTasks(int boardId)
    {
        var tasks = await _taskRepository
            .Where(r => r.BoardId == boardId)
            .Include(r => r.Project)
            .ToListAsync();

        return await ProcessTasksWithAssignees(tasks);
    }

    public async Task<List<TaskOutput>> ListProjectTasks(int projectId)
    {
        var tasks = await _taskRepository
            .Where(r => r.ProjectId == projectId)
            .Include(r => r.Project)
            .ToListAsync();

        return await ProcessTasksWithAssignees(tasks);
    }

    public async Task<List<TaskOutput>> SearchTasks(ClaimsPrincipal user, string text, int projectId)
    {
        text = text.ToLower().Trim();
        var userProjectIds = (await _userService.ListUserProjects(user)).Select(r => r.Id);

        if (projectId == 0)
        {
            return await ProcessTasksWithAssignees(await _taskRepository
                .Where(r => userProjectIds.Contains(r.ProjectId) &&
                            (r.Description.ToLower().Contains(text) ||
                             r.Title.ToLower().Contains(text) ||
                             text.Contains(r.Id.ToString()))
                )
                .ToListAsync());
        }

        return await ProcessTasksWithAssignees(await _taskRepository
            // TODO find a better way to incorporate [ValidProjectId]
            .Where(r => r.ProjectId == projectId && userProjectIds.Contains(projectId) &&
                        (r.Description.ToLower().Contains(text) ||
                         r.Title.ToLower().Contains(text) ||
                         text.Contains(r.Id.ToString()))
            )
            .ToListAsync());
    }

    // TODO: delete attachments as well
    public async Task DeleteTask(int userId, int taskId)
    {
        var task = await GetByIdInternal(taskId);
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

        // Break the parent relation
        foreach (var parentTask in parentTasks)
        {
            await UpdateTaskProperty(userId, new UpdateTaskPropertyInput
            {
                Params = new List<UpdateTaskPropertyInputParam>
                {
                    new UpdateTaskPropertyInputParam
                    {
                        Property = "ParentTaskItemId",
                        Value = null,
                    }
                },
                TaskId = parentTask.Id
            });
        }

        _dbContext.TaskComments.RemoveRange(comments);
        _dbContext.TaskRelations.RemoveRange(relations);
        _dbContext.TaskCustomFieldValues.RemoveRange(customFieldValues);
        _dbContext.TaskChangeLogs.RemoveRange(changeLogs);
        _dbContext.UserNotifications.RemoveRange(notifications);
        await _dbContext.SaveChangesAsync();
        await _taskRepository.Delete(task);
    }

    public async Task<TaskItem> GetByIdInternal(int taskId)
    {
        return await _taskRepository.GetByIdWithIncludes(taskId, t => t.Project);
    }

    public async Task<TaskOutput> GetById(int taskId)
    {
        var task = await GetByIdInternal(taskId);
        return MapTaskToOutput(task, await _taskAssigneeService.GetTaskAssignees(task.Id));
    }

    public async Task MoveTaskToColumn(int userId, MoveTaskInput input)
    {
        await UpdateTaskProperty(userId, new UpdateTaskPropertyInput
        {
            TaskId = input.TaskId,
            Params = new List<UpdateTaskPropertyInputParam>
            {
                new UpdateTaskPropertyInputParam
                {
                    Property = "BoardId",
                    Value = input.TargetBoardId.ToString(),
                },
                new UpdateTaskPropertyInputParam
                {
                    Property = "ColumnId",
                    Value = input.TargetColumnId.ToString(),
                }
            },
        });
    }

    public async Task UpdateTaskProperty(int userId, UpdateTaskPropertyInput input)
    {
        foreach (var param in input.Params)
        {
            var task = await GetByIdInternal(input.TaskId);
            var oldValue = ReflectionUtils.GetObjectPropertyValue(task, param.Property);
            var newValue = param.Value;

            // If old and new value are same, we don't need to update the task and create a changelog
            if (oldValue == newValue)
            {
                continue;
            }

            if (param.Property == "BoardId")
            {
                task.BoardId = Convert.ToInt32(newValue);
            }
            else if (param.Property == "ColumnId")
            {
                task.ColumnId = Convert.ToInt32(newValue);
            }
            else if (param.Property == "ParentTaskItemId")
            {
                task.ParentTaskItemId = Convert.ToInt32(newValue);
            }
            else if (param.Property == "ParentTaskItemId")
            {
                task.ParentTaskItemId = Convert.ToInt32(newValue);
            }
            else if (param.Property == "Order")
            {
                task.Order = Convert.ToInt32(newValue);
            }
            else if (param.Property == "Title")
            {
                task.Title = newValue;
            }
            else if (param.Property == "Description")
            {
                task.Description = newValue;
            }

            await _taskRepository.Update(task);
            await _taskChangeLogService.CreateChangeLog(userId, task, param.Property, oldValue, newValue);
        }
    }

    public async Task UpdateTaskAssigneeUser(int userId, UpdateTaskAssigneeUserInput input)
    {
        var task = await GetByIdInternal(input.TaskId);
        await _taskAssigneeService.SetTaskAssignees(userId, task, input.UserIds);
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

        // Operating user someone else, notify task assignee user
        foreach (var taskAssignee in await _taskAssigneeService.GetTaskAssigneesWithAllStakeholders(taskId))
        {
            const string type = "TASK_ATTACHMENT_UPLOADED";
            const string content = "A file is attached.";
            var byUserId = userId;
            var toUserId = taskAssignee;
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