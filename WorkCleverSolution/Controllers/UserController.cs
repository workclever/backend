using Microsoft.AspNetCore.Mvc;
using WorkCleverSolution.Attributes;
using WorkCleverSolution.Attributes.Validation;
using WorkCleverSolution.Dto.User;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Controllers;

[Route("Api/User/[action]")]
public class UserController : BaseApiController
{
    public UserController(IServices services) : base(services)
    {
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> GetUser()
    {
        return Wrap(await Services.UserService().GetUser(User));
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListUserProjects()
    {
        return Wrap(await Services.UserService().ListUserProjects(User));
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListAllUsers()
    {
        return Wrap(await Services.UserService().ListAllUsers());
    }

    [HttpPost]
    public async Task<ServiceResult> UpdateUser([FromBody] UpdateUserInput input)
    {
        if (User.IsAdmin() || User.GetUserId() == input.UserId)
        {
            await Services.UserService().UpdateUser(input);
        }

        return Wrap();
    }

    [HttpPost]
    [JwtAuthorize(Policy = "RequireAdminRole")]
    public async Task<ServiceResult> CreateUser([FromBody] CreateUserInput input)
    {
        await Services.UserService().CreateUser(input);
        return Wrap();
    }

    [HttpGet]
    [JwtAuthorize(Policy = "RequireAdminRole")]
    public async Task<ServiceResult> GetUserRoles([ValidUserId] int userId)
    {
        return Wrap(await Services.UserService().GetUserRoles(userId));
    }

    [HttpGet]
    [JwtAuthorize(Policy = "RequireAdminRole")]
    public async Task<ServiceResult> GetAllRoles()
    {
        return Wrap(await Services.UserService().GetAllRoles());
    }

    [HttpPost]
    [JwtAuthorize(Policy = "RequireAdminRole")]
    public async Task<ServiceResult> AddUserToRoles([FromBody] AddUserToRoleInput input)
    {
        await Services.UserService().AddUserToRoles(input);
        return Wrap();
    }

    [HttpGet]
    [JwtAuthorize(Policy = "RequireAdminRole")]
    public async Task<ServiceResult> GetUserAssignedProjectIds([ValidUserId] int userId)
    {
        return Wrap(await Services.UserService().GetUserAssignedProjectIds(userId));
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListMyAccessedEntities()
    {
        return Wrap(await Services.AccessManagerService().ListMyAccessedEntities(User.GetUserId()));
    }

    [HttpPost]
    public async Task<ServiceResult> UpdateUserPreference([FromBody] UpdateUserPreferenceInput input)
    {
        await Services.UserService().UpdateUserPreference(User.GetUserId(), input);
        return Wrap();
    }

    [HttpPost]
    public async Task<ServiceResult> ChangePassword([FromBody] ChangePasswordInput input)
    {
        await Services.UserService().ChangePassword(User.GetUserId(), input);
        return Wrap();
    }

    [HttpPost]
    public async Task<ServiceResult> ChangeAvatar([FromForm] ChangeAvatarInput input)
    {
        return Wrap(await Services.UserService().ChangeAvatar(User.GetUserId(), input));
    }
}