using System.Security.Claims;
using WorkCleverSolution.Data;
using WorkCleverSolution.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Dto.Auth;
using WorkCleverSolution.Dto.Project;
using WorkCleverSolution.Dto.User;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Utils;

namespace WorkCleverSolution.Services;

public interface IUserService
{
    Task<UserOutput> GetUser(ClaimsPrincipal identity);
    Task UpdateUser(UpdateUserInput input);
    Task CreateUser(CreateUserInput input);
    Task<User> GetUserInternal(int userId);
    Task<List<ProjectOutput>> ListUserProjects(ClaimsPrincipal identity);
    Task<List<AllUserOutput>> ListAllUsers();
    Task<IList<string>> GetUserRoles(int userId);
    Task<List<IdentityRole<int>>> GetAllRoles();
    Task AddUserToRoles(AddUserToRoleInput input);
    Task<List<int>> GetUserAssignedProjectIds(int userId);
    Task UpdateUserPreference(int userId, UpdateUserPreferenceInput input);
    Task ChangePassword(int userId, ChangePasswordInput input);
    Task<string> ChangeAvatar(int userId, ChangeAvatarInput input);
}

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly IRepository<Project> _projectRepository;
    private readonly IRepository<UserPreference> _userPreferenceRepository;
    private readonly IAuthService _authService;
    private readonly IUserEntityAccessManagerService _accessManagerService;
    private readonly IFileUploadService _fileUploadService;

    public UserService(
        UserManager<User> userManager,
        ApplicationDbContext dbContext, IAuthService authService,
        IUserEntityAccessManagerService accessManagerService,
        IFileUploadService fileUploadService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _authService = authService;
        _accessManagerService = accessManagerService;
        _fileUploadService = fileUploadService;
        _projectRepository = new Repository<Project>(dbContext);
        _userPreferenceRepository = new Repository<UserPreference>(dbContext);
    }

    private static UserOutput MapUserToOutput(User user, IList<string> roles, UserPreference userPreference)
    {
        return new UserOutput
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            Roles = roles,
            AvatarUrl = user.AvatarUrl,
            Preferences = new UserPreferenceOutput
            {
                Timezone = userPreference.Timezone
            }
        };
    }

    public async Task<UserOutput> GetUser(ClaimsPrincipal identity)
    {
        var user = await GetUserInternal(identity.GetUserId());
        var roles = await _userManager.GetRolesAsync(user);
        var preference = await _userPreferenceRepository
            .Where(r => r.UserId == user.Id)
            .SingleOrDefaultAsync();
        return MapUserToOutput(user, roles, preference);
    }

    public async Task UpdateUser(UpdateUserInput input)
    {
        var user = await GetUserInternal(input.UserId);
        user.FullName = input.FullName;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateUser(CreateUserInput input)
    {
        await _authService.RegisterAsync(new RegisterInput
        {
            Email = input.Email,
            Password = input.Password,
            FullName = input.FullName
        });
    }


    public async Task<User> GetUserInternal(int userId)
    {
        return await _dbContext.Users.FindAsync(userId);
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

    public async Task<List<ProjectOutput>> ListUserProjects(ClaimsPrincipal user)
    {
        if (user.IsAdmin())
        {
            return (await _projectRepository.GetAll())
                .Select(MapProjectToOutput)
                .ToList();
        }

        var ownedProjects = await _projectRepository
            .Where(r => r.OwnerUserId == user.GetUserId())
            .ToListAsync();

        var assignedProjectIds = await GetUserAssignedProjectIds(user.GetUserId());
        var assignedProjects = await _projectRepository
            .Where(r => assignedProjectIds.Contains(r.Id))
            .AsNoTracking()
            .ToListAsync();

        foreach (var project in ownedProjects)
        {
            if (!assignedProjects.Select(r => r.Id).Contains(project.Id))
            {
                assignedProjects.Add(project);
            }
        }

        return assignedProjects
            .Select(MapProjectToOutput)
            .ToList();
    }

    public async Task<List<AllUserOutput>> ListAllUsers()
    {
        return await _dbContext
            .Users
            .Select(r => new AllUserOutput
            {
                Id = r.Id,
                FullName = r.FullName,
                Email = r.Email,
                AvatarUrl = r.AvatarUrl
            })
            .ToListAsync();
    }

    public async Task<IList<string>> GetUserRoles(int userId)
    {
        var user = await GetUserInternal(userId);
        var roles = await _userManager.GetRolesAsync(user);
        return roles;
    }

    public async Task<List<IdentityRole<int>>> GetAllRoles()
    {
        return await _dbContext.Roles.ToListAsync();
    }

    public async Task AddUserToRoles(AddUserToRoleInput input)
    {
        var user = await GetUserInternal(input.UserId);
        var existingRoles = await GetUserRoles(input.UserId);
        await _userManager.RemoveFromRolesAsync(user, existingRoles);
        await _userManager.AddToRolesAsync(user, input.Roles.ToList());
    }

    public async Task<List<int>> GetUserAssignedProjectIds(int userId)
    {
        var accessedEntities = await _accessManagerService.ListMyAccessedEntities(userId);
        var assignedProjectIds =
            accessedEntities.Where(r => r.EntityClass == "Project").Select(r => r.EntityId).ToList();
        return assignedProjectIds;
    }

    public async Task UpdateUserPreference(int userId, UpdateUserPreferenceInput input)
    {
        var userPreference = await _userPreferenceRepository
            .Where(r => r.UserId == userId)
            .SingleOrDefaultAsync();
        var oldValue = ReflectionUtils.GetObjectPropertyValue(userPreference, input.Property);
        var newValue = input.Value;

        // If old and new value are same, we don't need to update
        if (oldValue == newValue)
        {
            return;
        }

        ReflectionUtils.SetObjectProperty(userPreference, input.Property, input.Value);
        await _userPreferenceRepository.Update(userPreference);
    }

    public async Task ChangePassword(int userId, ChangePasswordInput input)
    {
        var user = await GetUserInternal(userId);
        var result = await _userManager.ChangePasswordAsync(user, input.OldPassword, input.NewPassword);

        if (result.Succeeded)
        {
            return;
        }

        var errors = result.Errors.Select(r => r.Description).ToList();
        throw new ApplicationException(string.Join(", ", errors.ToArray()));
    }

    public async Task<string> ChangeAvatar(int userId, ChangeAvatarInput input)
    {
        var fileDiskName = Guid.NewGuid().ToString();
        var accessUrl = await _fileUploadService.UploadFile(
            fileDiskName,
            new[] { "avatars", userId.ToString() },
            input.file
        );
        var user = await GetUserInternal(userId);
        user.AvatarUrl = accessUrl;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return accessUrl;
    }
}