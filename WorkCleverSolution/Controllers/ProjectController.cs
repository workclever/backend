using Microsoft.AspNetCore.Mvc;
using WorkCleverSolution.Attributes;
using WorkCleverSolution.Attributes.Authorization;
using WorkCleverSolution.Attributes.Validation;
using WorkCleverSolution.Data.Identity;
using WorkCleverSolution.Dto.Project;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Controllers;

[Route("Api/Project/[action]")]
public class ProjectController : BaseApiController
{
    public ProjectController(IServices services) : base(services)
    {
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListProjectUsers([ValidProjectId] int projectId)
    {
        return Wrap(await Services.ProjectService().ListProjectUsers(projectId));
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> CreateProjectAssignee([FromBody] CreateProjectAssigneeInput input)
    {
        await Services.ProjectService().CreateProjectAssignee(input);
        return Wrap();
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> RemoveProjectAssignee([FromBody] CreateProjectAssigneeInput input)
    {
        await Services.ProjectService().RemoveProjectAssignee(input);
        return Wrap();
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> CreateProject([FromBody] CreateProjectInput input)
    {
        return Wrap(await Services.ProjectService().CreateProject(User.GetUserId(), input));
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> UpdateProject([FromBody] UpdateProjectInput input)
    {
        await Services.ProjectService().UpdateProject(User.GetUserId(), input);
        return Wrap();
    }

    [HttpDelete]
    [JwtAuthorize]
    public async Task<ServiceResult> DeleteProject([ValidProjectId] [ManageableProjectId] int projectId)
    {
        await Services.ProjectService().DeleteProject(User.GetUserId(), projectId);
        return Wrap();
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> GetProject([ValidProjectId] int projectId)
    {
        return Wrap(await Services.ProjectService().GetById(projectId));
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListProjectUserAccesses([ValidProjectId] [ManageableProjectId] int projectId)
    {
        return Wrap(await Services.AccessManagerService().ListAccessedUsersByEntityId(projectId, "Project"));
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> CreateManagerUserForProject(
        [FromBody] CreateManagerUserForProjectInput input)
    {
        await Services.AccessManagerService()
            .CreateUserEntityAccess(input.UserId, Permissions.CanManageProject, input.ProjectId, "Project");
        return Wrap();
    }

    [HttpDelete]
    [JwtAuthorize]
    public async Task<ServiceResult> DeleteManagerUserForProject(
        [FromBody] CreateManagerUserForProjectInput input)
    {
        await Services.AccessManagerService()
            .DeleteUserEntityAccess(input.UserId, Permissions.CanManageProject, input.ProjectId, "Project");
        return Wrap();
    }
}