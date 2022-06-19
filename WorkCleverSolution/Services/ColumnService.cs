using WorkCleverSolution.Data;
using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Dto.Project.Board;
using WorkCleverSolution.Dto.Project.Column;

namespace WorkCleverSolution.Services;

public interface IColumnService
{
    Task<ColumnOutput> CreateColumn(int userId, CreateColumnInput input);
    Task UpdateBoardColumn(int userId, UpdateColumnInput input);
    Task<List<ColumnOutput>> ListBoardColumns(int boardId);
    Task DeleteColumn(int userId, int columnId);
    Task<Column> GetById(int boardId);
    Task UpdateColumnOrders(UpdateColumnOrdersInput input);
}

public class ColumnService : IColumnService
{
    private readonly IRepository<Column> _columnRepository;
    private readonly ITaskService _taskService;
    private readonly ApplicationDbContext _dbContext;

    public ColumnService(ApplicationDbContext dbContext, ITaskService taskService)
    {
        _dbContext = dbContext;
        _taskService = taskService;
        _columnRepository = new Repository<Column>(dbContext);
    }

    private static ColumnOutput MapColumnToOutput(Column column)
    {
        return new ColumnOutput
        {
            Id = column.Id,
            Name = column.Name,
            BoardId = column.BoardId,
            Hidden = column.Hidden,
            Order = column.Order,
            Color = column.Color
        };
    }

    public async Task<ColumnOutput> CreateColumn(int userId, CreateColumnInput input)
    {
        var lastOrder = await _columnRepository
            .Where(r => r.BoardId == input.BoardId)
            .OrderByDescending(r => r.Order)
            .Select(r => r.Order)
            .FirstOrDefaultAsync();

        var column = new Column
        {
            Name = input.Name,
            ProjectId = input.ProjectId,
            BoardId = input.BoardId,
            UserId = userId,
            Hidden = input.Hidden,
            Order = lastOrder == 0 ? 1 : lastOrder + 1,
            Color = "magenta"
        };

        await _columnRepository.Create(column);
        return MapColumnToOutput(column);
    }

    public async Task UpdateBoardColumn(int userId, UpdateColumnInput input)
    {
        var column = await GetById(input.ColumnId);
        column.Name = input.Name;
        column.Hidden = input.Hidden;
        column.Color = input.Color;
        await _columnRepository.Update(column);
    }

    public async Task<List<ColumnOutput>> ListBoardColumns(int boardId)
    {
        return await _columnRepository
            .Where(r => r.BoardId == boardId)
            .Select(r => MapColumnToOutput(r))
            .ToListAsync();
    }

    public async Task DeleteColumn(int userId, int columnId)
    {
        var column = await GetById(columnId);
        if (column != null)
        {
            await _columnRepository.Delete(column);
        }

        var taskIdsInColumn = await _dbContext
            .TaskItems
            .Where(r => r.ColumnId == columnId)
            .Select(r => r.Id)
            .ToListAsync();

        foreach (var taskId in taskIdsInColumn)
        {
            await _taskService.DeleteTask(userId, taskId);
        }
    }

    public async Task<Column> GetById(int columnId)
    {
        return await _columnRepository.GetById(columnId);
    }

    public async Task UpdateColumnOrders(UpdateColumnOrdersInput input)
    {
        var order = 1;
        foreach (var columnId in input.ColumnIds)
        {
            var column = await _columnRepository
                .Where(r => r.Id == columnId && r.BoardId == input.BoardId)
                .SingleOrDefaultAsync();
            if (column != null)
            {
                column.Order = order;
                await _columnRepository.Update(column);
                order++;
            }
        }
    }
}