using Microsoft.AspNetCore.Mvc;
using WorkCleverSolution.Attributes;
using WorkCleverSolution.Dto.Global;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Controllers;

[Route("Api/TaskRelationTypeDef/[action]")]
public class TaskRelationTypeDefController : BaseApiController
{
    public TaskRelationTypeDefController(IServices services) : base(services)
    {
    }

    [HttpGet]
    public async Task<ServiceResult> ListTaskRelationTypeDefs()
    {
        return Wrap(await Services.TaskRelationTypeDefService().ListTaskRelationTypeDefs());
    }

    [HttpPost]
    [JwtAuthorize(Policy = "RequireAdminRole")]
    public async Task<ServiceResult> CreateTaskRelationTypeDef([FromBody] CreateTaskRelationTypeDefInput input)
    {
        await Services.TaskRelationTypeDefService().CreateTaskRelationTypeDef(input);
        return Wrap();
    }

    [HttpPost]
    [JwtAuthorize(Policy = "RequireAdminRole")]
    public async Task<ServiceResult> UpdateTaskRelationTypeDef([FromBody] UpdateTaskRelationTypeDefInput input)
    {
        await Services.TaskRelationTypeDefService().UpdateTaskRelationTypeDef(input);
        return Wrap();
    }

    [HttpDelete]
    [JwtAuthorize(Policy = "RequireAdminRole")]
    public async Task<ServiceResult> DeleteTaskRelationTypeDef(int taskRelationTypeDefId)
    {
        await Services.TaskRelationTypeDefService().DeleteTaskRelationTypeDef(taskRelationTypeDefId);
        return Wrap();
    }
}