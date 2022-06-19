using Microsoft.AspNetCore.Mvc;
using WorkCleverSolution.Attributes;
using WorkCleverSolution.Attributes.Validation;
using WorkCleverSolution.Dto.Project.Comment;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Controllers;

[Route("Api/TaskComment/[action]")]
public class TaskCommentController : BaseApiController
{
    public TaskCommentController(IServices services) : base(services)
    {
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListTaskCommentsByBoardId([ValidBoardId] int boardId)
    {
        return Wrap(await Services.TaskCommentService().ListTaskCommentsByBoardId(boardId));
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> CreateTaskComment([FromBody] CreateTaskCommentInput input)
    {
        await Services.TaskCommentService().CreateTaskComment(User.GetUserId(), input);
        return Wrap();
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> UpdateTaskComment([FromBody] UpdateTaskCommentInput input)
    {
        await Services.TaskCommentService().UpdateTaskComment(User.GetUserId(), input);
        return Wrap();
    }

    [HttpDelete]
    [JwtAuthorize]
    public async Task<ServiceResult> DeleteTaskComment([FromBody] DeleteTaskCommentInput input)
    {
        await Services.TaskCommentService().DeleteTaskComment(User.GetUserId(), input);
        return Wrap();
    }
}