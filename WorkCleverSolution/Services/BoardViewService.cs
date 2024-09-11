using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Data;

namespace WorkCleverSolution.Services;

public interface IBoardViewService
{
    Task<List<BoardView>> ListBoardViewsByBoardId(int boardId);
    Task CreateBoardView(int boardId, string type);
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
    
    public async Task CreateBoardView(int boardId, string type)
    {
        await _boardViewRepository.Create(new BoardView()
        {
            BoardId = boardId,
            Config = new BoardViewConfig(type, new List<int>())
        });
    }
}