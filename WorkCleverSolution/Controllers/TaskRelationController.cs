using Microsoft.AspNetCore.Mvc;
using WorkCleverSolution.Attributes;
using WorkCleverSolution.Attributes.Validation;
using WorkCleverSolution.Dto.Project.Task;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Controllers;

[Route("Api/TaskRelation/[action]")]
public class TaskRelationController : BaseApiController
{
    public TaskRelationController(IServices services) : base(services)
    {
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> CreateTaskRelation([FromBody] CreateTaskRelationInput input)
    {
        await Services.TaskRelationService().CreateTaskRelation(User.GetUserId(), input);
        return Wrap();
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> UpdateTaskRelation([FromBody] UpdateTaskRelationInput input)
    {
        await Services.TaskRelationService().UpdateTaskRelation(User.GetUserId(), input);
        return Wrap();
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListTaskRelations([ValidTaskId] int taskId)
    {
        return Wrap(await Services.TaskRelationService().ListTaskRelations(taskId));
    }

    [HttpDelete]
    [JwtAuthorize]
    public async Task<ServiceResult> DeleteTaskRelation([ValidTaskParentRelationId] int taskParentRelationId)
    {
        await Services.TaskRelationService().DeleteTaskRelation(taskParentRelationId);
        return Wrap();
    }
}