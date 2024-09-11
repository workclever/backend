using Microsoft.AspNetCore.Mvc;
using WorkCleverSolution.Attributes;
using WorkCleverSolution.Attributes.Authorization;
using WorkCleverSolution.Attributes.Validation;
using WorkCleverSolution.Dto.Project.Board;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Controllers;

[Route("Api/Board/[action]")]
public class BoardController : BaseApiController
{
    public BoardController(IServices services) : base(services)
    {
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListAllBoards()
    {
        return Wrap(await Services.BoardService().ListAllBoards(User));
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> GetBoard([ValidBoardId] int boardId)
    {
        return Wrap(await Services.BoardService().GetBoard(boardId));
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> CreateBoard([FromBody] CreateBoardInput input)
    {
        return Wrap(await Services.BoardService().CreateBoard(User.GetUserId(), input));
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> UpdateBoard([FromBody] UpdateBoardInput input)
    {
        await Services.BoardService().UpdateBoard(User.GetUserId(), input);
        return Wrap();
    }

    [HttpDelete]
    [JwtAuthorize]
    public async Task<ServiceResult> DeleteBoard([ValidBoardId] [ManageableBoardId] int boardId)
    {
        await Services.BoardService().DeleteBoard(User.GetUserId(), boardId);
        return Wrap();
    }
    
    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListBoardViewsByBoardId([ValidBoardId] int boardId)
    {
        return Wrap(await Services.BoardViewService().ListBoardViewsByBoardId(boardId));
    }
    
    
    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> CreateBoardView([FromBody] CreateBoardViewInput input)
    {
        await Services.BoardViewService().CreateBoardView(input.BoardId, input.Type);
        return Wrap();
    }
}