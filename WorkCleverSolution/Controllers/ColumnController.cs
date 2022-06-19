using Microsoft.AspNetCore.Mvc;
using WorkCleverSolution.Attributes;
using WorkCleverSolution.Attributes.Authorization;
using WorkCleverSolution.Attributes.Validation;
using WorkCleverSolution.Dto.Project.Board;
using WorkCleverSolution.Dto.Project.Column;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Controllers;

[Route("Api/Column/[action]")]
public class ColumnController : BaseApiController
{
    public ColumnController(IServices services) : base(services)
    {
    }

    // Board column stuff
    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> CreateBoardColumn([FromBody] CreateColumnInput input)
    {
        return Wrap(await Services.ColumnService().CreateColumn(User.GetUserId(), input));
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> UpdateBoardColumn([FromBody] UpdateColumnInput input)
    {
        await Services.ColumnService().UpdateBoardColumn(User.GetUserId(), input);
        return Wrap();
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListBoardColumns([ValidBoardId] int boardId)
    {
        return Wrap(
            await Services.ColumnService().ListBoardColumns(boardId));
    }

    [HttpDelete]
    [JwtAuthorize]
    public async Task<ServiceResult> DeleteColumn([ValidColumnId] [ManageableColumnId] int columnId)
    {
        await Services.ColumnService().DeleteColumn(User.GetUserId(), columnId);
        return Wrap();
    }


    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> UpdateColumnOrders([FromBody] UpdateColumnOrdersInput input)
    {
        await Services.ColumnService().UpdateColumnOrders(input);
        return Wrap();
    }
}