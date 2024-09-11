namespace WorkCleverSolution.Services;

public interface IServices
{
    IAuthService AuthService();
    IUserService UserService();
    IUserNotificationService UserNotificationService();
    IProjectService ProjectService();
    IBoardService BoardService();
    IBoardViewService BoardViewService();
    IColumnService ColumnService();
    ITaskService TaskService();
    ITaskCommentService TaskCommentService();
    ITaskRelationService TaskRelationService();
    ITaskChangeLogService TaskChangeLogService();
    IUserEntityAccessManagerService AccessManagerService();
    ITaskRelationTypeDefService TaskRelationTypeDefService();
    ISiteSettingsService SiteSettingsService();
    ICustomFieldService CustomFieldService();
}

public class Services : IServices
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly IProjectService _projectService;
    private readonly IBoardService _boardService;
    private readonly IBoardViewService _boardViewService;
    private readonly IColumnService _columnService;
    private readonly ITaskService _taskService;
    private readonly ITaskCommentService _taskCommentService;
    private readonly ITaskRelationService _taskRelationService;
    private readonly ITaskChangeLogService _taskChangeLogService;
    private readonly IUserEntityAccessManagerService _accessManagerService;
    private readonly ITaskRelationTypeDefService _taskRelationTypeDefService;
    private readonly ITaskAssigneeService _taskAssigneeService;
    private readonly ISiteSettingsService _siteSettingsService;
    private readonly ICustomFieldService _customFieldService;
    private readonly IUserNotificationService _userNotificationService;

    public Services(
        IAuthService authService,
        IUserService userService,
        IProjectService projectService,
        IBoardService boardService,
        IBoardViewService boardViewService,
        ITaskService taskService,
        ITaskCommentService taskCommentService,
        ITaskRelationService taskRelationService,
        IColumnService columnService,
        IUserEntityAccessManagerService accessManagerService,
        ITaskRelationTypeDefService taskRelationTypeDefService,
        ISiteSettingsService siteSettingsService,
        ICustomFieldService customFieldService,
        IUserNotificationService userNotificationService,
        ITaskAssigneeService taskAssigneeService,
        ITaskChangeLogService taskChangeLogService)
    {
        _authService = authService;
        _userService = userService;
        _projectService = projectService;
        _boardService = boardService;
        _boardViewService = boardViewService;
        _taskService = taskService;
        _taskCommentService = taskCommentService;
        _taskRelationService = taskRelationService;
        _columnService = columnService;
        _accessManagerService = accessManagerService;
        _taskRelationTypeDefService = taskRelationTypeDefService;
        _siteSettingsService = siteSettingsService;
        _customFieldService = customFieldService;
        _userNotificationService = userNotificationService;
        _taskAssigneeService = taskAssigneeService;
        _taskChangeLogService = taskChangeLogService;
    }

    public IAuthService AuthService()
    {
        return _authService;
    }

    public IUserService UserService()
    {
        return _userService;
    }

    public IUserNotificationService UserNotificationService()
    {
        return _userNotificationService;
    }

    public IProjectService ProjectService()
    {
        return _projectService;
    }

    public IBoardService BoardService()
    {
        return _boardService;
    }

    public IBoardViewService BoardViewService()
    {
        return _boardViewService;
    }

    public IColumnService ColumnService()
    {
        return _columnService;
    }

    public ITaskService TaskService()
    {
        return _taskService;
    }

    public ITaskCommentService TaskCommentService()
    {
        return _taskCommentService;
    }

    public ITaskRelationService TaskRelationService()
    {
        return _taskRelationService;
    }

    public ITaskChangeLogService TaskChangeLogService()
    {
        return _taskChangeLogService;
    }

    public IUserEntityAccessManagerService AccessManagerService()
    {
        return _accessManagerService;
    }

    public ITaskRelationTypeDefService TaskRelationTypeDefService()
    {
        return _taskRelationTypeDefService;
    }

    public ISiteSettingsService SiteSettingsService()
    {
        return _siteSettingsService;
    }

    public ICustomFieldService CustomFieldService()
    {
        return _customFieldService;
    }
    
    public ITaskAssigneeService TaskAssigneeService()
    {
        return _taskAssigneeService;
    }
}