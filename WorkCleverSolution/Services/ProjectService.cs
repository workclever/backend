using System.Security.Claims;
using Microsoft.Data.Sqlite;
using WorkCleverSolution.Data;
using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Data.Identity;
using WorkCleverSolution.Dto.Project;
using WorkCleverSolution.Dto.Project.Board;

namespace WorkCleverSolution.Services;

public interface IProjectService
{
    Task<Project> CreateProject(int userId, CreateProjectInput input);
    Task UpdateProject(int userId, UpdateProjectInput input);
    Task DeleteProject(int userId, int projectId);
    Task<ProjectOutput> GetById(int projectId);
    Task<Project> GetByIdInternal(int projectId);
    Task<bool> HasAccess(ClaimsPrincipal user, int projectId);
    Task<List<ProjectUserOutput>> ListProjectUsers(int projectId);
    Task CreateProjectAssignee(CreateProjectAssigneeInput input);
    Task RemoveProjectAssignee(CreateProjectAssigneeInput input);
}

public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRepository<Project> _projectRepository;
    private readonly IUserService _userService;
    private readonly IBoardService _boardService;
    private readonly IUserEntityAccessManagerService _accessManagerService;

    public ProjectService(ApplicationDbContext dbContext,
        IUserService userService,
        IBoardService boardService,
        IUserEntityAccessManagerService accessManagerService)
    {
        _dbContext = dbContext;
        _userService = userService;
        _boardService = boardService;
        _accessManagerService = accessManagerService;
        _projectRepository = new Repository<Project>(dbContext);
    }


    public async Task<Project> CreateProject(int userId, CreateProjectInput input)
    {
        var project = new Project
        {
            Name = input.Name,
            OwnerUserId = userId,
            Slug = input.Slug.Trim().ToUpper()
        };

        try
        {
            await _projectRepository.Create(project);
            await _accessManagerService.CreateUserEntityAccess(userId, Permissions.CanManageProject, project.Id,
                "Project");
            await _boardService.CreateBoard(userId, new CreateBoardInput()
            {
                Name = "\ud83d\ude80 First board",
                ProjectId = project.Id
            });
        }
        catch (Exception e)
        {
            if (e.InnerException is SqliteException)
            {
                var ex = e.InnerException as SqliteException;
                if (ex.SqliteErrorCode == 19)
                {
                    throw new ApplicationException($"The {input.Slug} slug is already used in another project");
                }
            }

            throw;
        }

        return project;
    }

    public async Task UpdateProject(int userId, UpdateProjectInput input)
    {
        var project = await _projectRepository.GetById(input.ProjectId);
        project.Name = input.Name;
        project.Slug = input.Slug.Trim().ToUpper();
        await _projectRepository.Update(project);
    }

    public async Task DeleteProject(int userId, int projectId)
    {
        var project = await GetByIdInternal(projectId);
        await _projectRepository.Delete(project);
        var boardsInProject = await _dbContext
            .Boards
            .Where(r => r.ProjectId == projectId)
            .Select(r => r.Id)
            .ToListAsync();

        foreach (var boardId in boardsInProject)
        {
            await _boardService.DeleteBoard(userId, boardId);
        }

        // Cleanup entity access for project and users
        var entityClass = "Project";
        await _accessManagerService.DeleteAllEntityAccessByEntityId(projectId, entityClass);
    }

    private static ProjectOutput MapProjectToOutput(Project project)
    {
        return new ProjectOutput
        {
            Id = project.Id,
            Name = project.Name,
            Slug = project.Slug,
        };
    }

    public async Task<ProjectOutput> GetById(int projectId)
    {
        var project = await GetByIdInternal(projectId);
        if (project == null)
        {
            throw new ApplicationException("PROJECT_NOT_FOUND");
        }

        return MapProjectToOutput(project);
    }

    public async Task<Project> GetByIdInternal(int projectId)
    {
        return await _projectRepository.GetById(projectId);
    }

    public async Task<bool> HasAccess(ClaimsPrincipal user, int projectId)
    {
        var userProjects = await _userService.ListUserProjects(user);
        return userProjects.Select(r => r.Id).Contains(projectId);
    }

    public async Task<List<ProjectUserOutput>> ListProjectUsers(int projectId)
    {
        var accessedUsers = await _accessManagerService.ListAccessedUsersByEntityId(projectId, "Project");

        return await _dbContext
            .Users
            .Where(r => accessedUsers.Select(r => r.UserId).Contains(r.Id))
            .Select(r => new ProjectUserOutput
            {
                Id = r.Id,
                FullName = r.FullName,
                Email = r.Email,
                AvatarUrl = r.AvatarUrl,
            }).ToListAsync();
    }

    public async Task CreateProjectAssignee(CreateProjectAssigneeInput input)
    {
        foreach (var dto in input.Ids)
        {
            var entityClass = "Project";
            await _accessManagerService.CreateUserEntityAccess(dto.UserId, Permissions.CollaborateProject,
                input.ProjectId, entityClass);
        }
    }

    public async Task RemoveProjectAssignee(CreateProjectAssigneeInput input)
    {
        foreach (var dto in input.Ids)
        {
            var entityClass = "Project";
            await _accessManagerService.DeleteUserEntityAccess(dto.UserId,
                Permissions.CollaborateProject, input.ProjectId, entityClass);
        }
    }
}