using System.Security.Claims;
using WorkCleverSolution.Data;
using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Dto.Project.Board;

namespace WorkCleverSolution.Services;

public interface IBoardService
{
    Task<BoardOutput> CreateBoard(int userId, CreateBoardInput input);
    Task UpdateBoard(int userId, UpdateBoardInput input);
    Task<List<BoardOutput>> ListAllBoards(ClaimsPrincipal user);
    Task DeleteBoard(int userId, int boardId);
    Task<Board> GetById(int boardId);
    Task<BoardOutput> GetBoard(int boardId);
}

public class BoardService : IBoardService
{
    private readonly IRepository<Board> _boardRepository;
    private readonly IColumnService _columnService;
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly IBoardViewService _boardViewService;

    public BoardService(
        ApplicationDbContext dbContext, 
        IColumnService columnService, 
        IUserService userService, 
        IBoardViewService boardViewService)
    {
        _dbContext = dbContext;
        _columnService = columnService;
        _userService = userService;
        _boardViewService = boardViewService;
        _boardRepository = new Repository<Board>(dbContext);
    }

    private static BoardOutput MapProjectBoardToOutput(Board board)
    {
        return new BoardOutput
        {
            Id = board.Id,
            Name = board.Name,
            ProjectId = board.ProjectId
        };
    }

    public async Task<BoardOutput> CreateBoard(int userId, CreateBoardInput input)
    {
        var board = new Board
        {
            Name = input.Name,
            ProjectId = input.ProjectId,
            UserId = userId
        };

        await _boardRepository.Create(board);
        await _boardViewService.CreateBoardView(userId, new CreateBoardViewInput()
        {
            BoardId = board.Id,
            Name = "Kanban",
            Type = "kanban"
        });
        await _boardViewService.CreateBoardView(userId, new CreateBoardViewInput()
        {
            BoardId = board.Id,
            Name = "Tree",
            Type = "tree"
        });

        return MapProjectBoardToOutput(board);
    }

    public async Task UpdateBoard(int userId, UpdateBoardInput input)
    {
        var board = await _boardRepository.GetById(input.BoardId);
        board.Name = input.Name;
        await _boardRepository.Update(board);
    }

    public async Task<List<BoardOutput>> ListAllBoards(ClaimsPrincipal user)
    {
        var userProjects = await _userService.ListUserProjects(user);
        var boards = await _boardRepository
            .Where(r => userProjects.Select(r => r.Id).Contains(r.ProjectId))
            .Select(r => MapProjectBoardToOutput(r))
            .ToListAsync();
        return boards;
    }

    public async Task DeleteBoard(int userId, int boardId)
    {
        var board = await GetById(boardId);
        await _boardRepository.Delete(board);

        var columnIdsInBoard = await _dbContext
            .Columns
            .Where(r => r.BoardId == boardId)
            .Select(r => r.Id)
            .ToListAsync();

        foreach (var columnId in columnIdsInBoard)
        {
            await _columnService.DeleteColumn(userId, columnId);
        }
    }

    public async Task<Board> GetById(int boardId)
    {
        return await _boardRepository.GetById(boardId);
    }

    public async Task<BoardOutput> GetBoard(int boardId)
    {
        return MapProjectBoardToOutput(await GetById(boardId));
    }
}