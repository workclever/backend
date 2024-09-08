using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WorkCleverSolution.Attributes;
using WorkCleverSolution.Attributes.Validation;
using WorkCleverSolution.Dto.Project.Task;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Controllers;

[Route("Api/Task/[action]")]
public class TaskController : BaseApiController
{
    public TaskController(IServices services) : base(services)
    {
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListProjectTasks([ValidProjectId] int projectId)
    {
        return Wrap(await Services.TaskService().ListProjectTasks(projectId));
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> SearchTasks([Required] string text, int projectId)
    {
        return Wrap(await Services.TaskService().SearchTasks(User, text, projectId));
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListTaskChangeLog([ValidTaskId] int taskId)
    {
        return Wrap(await Services.TaskChangeLogService().ListTaskChangeLog(taskId));
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> CreateTask([FromBody] CreateTaskInput input)
    {
        return Wrap(await Services.TaskService().CreateTask(User.GetUserId(), input));
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> GetTask([ValidTaskId] int taskId)
    {
        return Wrap(await Services.TaskService().GetById(taskId));
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> MoveTaskToColumn([FromBody] MoveTaskInput input)
    {
        await Services.TaskService().MoveTaskToColumn(User.GetUserId(), input);
        return Wrap();
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> UpdateTaskAssigneeUser(
        [FromBody] UpdateTaskAssigneeUserInput input)
    {
        await Services.TaskService().UpdateTaskAssigneeUser(User.GetUserId(), input);
        return Wrap();
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> UpdateTaskProperty(
        [FromBody] UpdateTaskPropertyInput input)
    {
        await Services.TaskService().UpdateTaskProperty(User.GetUserId(), input);
        return Wrap();
    }

    [HttpDelete]
    [JwtAuthorize]
    public async Task<ServiceResult> DeleteTask([ValidTaskId] int taskId)
    {
        await Services.TaskService().DeleteTask(User.GetUserId(), taskId);
        return Wrap();
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> CreateSubtaskRelation([ValidTaskId] int parentTaskItemId, [ValidTaskId] int taskId)
    {
        await Services.TaskService().CreateSubtaskRelation(parentTaskItemId, taskId);
        return Wrap();
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> UploadTaskAttachmentInput([FromForm] UploadAttachmentInput input,
        [ValidTaskId] int taskId)
    {
        await Services.TaskService().UploadTaskAttachmentInput(User.GetUserId(), taskId, input);
        return Wrap();
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListTaskAttachments([ValidTaskId] int taskId)
    {
        return Wrap(await Services.TaskService().ListTaskAttachments(taskId));
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> UpdateTaskOrders([FromBody] UpdateTaskOrdersInput input)
    {
        await Services.TaskService().UpdateTaskOrders(input);
        return Wrap();
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> SendTaskToTopOrBottom([FromBody] SendTaskToTopOrBottomInput input)
    {
        await Services.TaskService().SendTaskToTopOrBottom(input);
        return Wrap();
    }
}