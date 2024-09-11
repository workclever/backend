using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Data;
using WorkCleverSolution.Dto.Project.Board;

namespace WorkCleverSolution.Services;

public interface IBoardViewService
{
    Task<List<BoardView>> ListBoardViewsByBoardId(int boardId);
    
    Task<BoardView> GetById(int boardViewId);
    Task<BoardView> CreateBoardView(int userId, CreateBoardViewInput input);
    Task UpdateBoardView(int userId, UpdateBoardViewInput input);
    Task DeleteBoardView(int userId, int boardViewId);
}

public class BoardViewService : IBoardViewService
{
    private readonly IRepository<BoardView> _boardViewRepository;

    public BoardViewService(ApplicationDbContext dbContext)
    {
        _boardViewRepository = new Repository<BoardView>(dbContext);
    }

    public async Task<List<BoardView>> ListBoardViewsByBoardId(int boardId)
    {
        return await _boardViewRepository.Where(r => r.BoardId == boardId).ToListAsync();
    }

    public async Task<BoardView> GetById(int boardViewId)
    {
        return await _boardViewRepository.GetById(boardViewId);
    }

    public async Task<BoardView> CreateBoardView(int userId, CreateBoardViewInput input)
    {
        var boardView = await _boardViewRepository.Create(new BoardView()
        {
            UserId = userId,
            BoardId = input.BoardId,
            Config = new BoardViewConfig(input.Type, input.Name, new List<int>())
        });

        return boardView;
    }

    public async Task UpdateBoardView(int userId, UpdateBoardViewInput input)
    {
        var boardView = await GetById(input.BoardViewId);
        if (boardView == null)
        {
            return;
        }

        boardView.Config = new BoardViewConfig(boardView.Config.Type, input.Name, input.VisibleCustomFields);
        await _boardViewRepository.Update(boardView);
    }

    public async Task DeleteBoardView(int userId, int boardViewId)
    {
        var boardView = await GetById(boardViewId);
        if (boardView == null)
        {
            return;
        }

        await _boardViewRepository.Delete(boardView);
    }
}