using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Data;
using WorkCleverSolution.Dto.Project.CustomField;

namespace WorkCleverSolution.Services;

public interface ICustomFieldService
{
    Task CreateCustomField(CreateCustomFieldInput input);
    Task DeleteCustomField(int projectId, int customFieldId);
    Task UpdateCustomField(UpdateCustomFieldInput input);
    Task<List<CustomField>> ListCustomFields(int projectId);

    Task<Dictionary<int, Dictionary<int, dynamic>>> ListTaskCustomFieldValuesByBoard(int boardId);

    Task CreateCustomFieldTaskValue(CreateCustomFieldTaskValueInput input);
}

public class CustomFieldService : ICustomFieldService
{
    private readonly IRepository<CustomField> _customFieldRepository;
    private readonly IRepository<CustomFieldSelectOption> _customFieldSelectOptionRepository;
    private readonly IRepository<TaskCustomFieldValue> _taskCustomFieldValueRepository;
    private readonly ITaskService _taskService;
    private readonly IBoardService _boardService;

    public CustomFieldService(ApplicationDbContext dbContext, ITaskService taskService, IBoardService boardService)
    {
        _taskService = taskService;
        _boardService = boardService;
        _customFieldRepository = new Repository<CustomField>(dbContext);
        _customFieldSelectOptionRepository = new Repository<CustomFieldSelectOption>(dbContext);
        _taskCustomFieldValueRepository = new Repository<TaskCustomFieldValue>(dbContext);
    }

    public async Task CreateCustomField(CreateCustomFieldInput input)
    {
        var customField = new CustomField
        {
            ProjectId = input.ProjectId,
            FieldType = input.FieldType,
            FieldName = input.FieldName,
            Enabled = input.Enabled,
            ShowInTaskCard = input.ShowInTaskCard,
        };
        await _customFieldRepository.Create(customField);

        if (input.SelectOptions == null)
        {
            return;
        }

        foreach (var option in input.SelectOptions)
        {
            var selectOption = new CustomFieldSelectOption
            {
                CustomFieldId = customField.Id,
                Name = option.Name,
                Color = option.Color
            };
            await _customFieldSelectOptionRepository.Create(selectOption);
        }
    }

    public async Task DeleteCustomField(int projectId, int customFieldId)
    {
        var customField = await _customFieldRepository
            .Where(r => r.ProjectId == projectId && r.Id == customFieldId)
            .SingleOrDefaultAsync();

        if (customField == null)
        {
            return;
        }

        await _customFieldRepository.Delete(customField);

        // Delete all relevant options as well
        var selectOptions = await _customFieldSelectOptionRepository
            .Where(r => r.CustomFieldId == customFieldId)
            .ToListAsync();

        await _customFieldSelectOptionRepository.DeleteRange(selectOptions);
    }

    public async Task UpdateCustomField(UpdateCustomFieldInput input)
    {
        var customField = await _customFieldRepository
            .Where(r => r.Id == input.CustomFieldId)
            .SingleOrDefaultAsync();

        if (customField == null)
        {
            throw new ApplicationException("CUSTOM_FIELD_NOT_FOUND");
        }

        customField.FieldType = input.FieldType;
        customField.FieldName = input.FieldName;
        customField.ShowInTaskCard = input.ShowInTaskCard;
        customField.Enabled = input.Enabled;
        await _customFieldRepository.Update(customField);

        if (input.SelectOptions == null)
        {
            return;
        }

        var fromDbOptions = await _customFieldSelectOptionRepository
            .Where(r => r.CustomFieldId == customField.Id)
            .ToListAsync();
        var newOptionIds = input.SelectOptions.Select(r => r.Id).ToList();

        foreach (var fromDbOption in fromDbOptions)
        {
            // the id is deleted by user input
            if (!newOptionIds.Contains(fromDbOption.Id))
            {
                await _customFieldSelectOptionRepository.Delete(fromDbOption);
            }
        }

        foreach (var option in input.SelectOptions)
        {
            var selectOption = await _customFieldSelectOptionRepository.GetById(option.Id);
            if (selectOption != null)
            {
                selectOption.Name = option.Name;
                selectOption.Color = option.Color;
                await _customFieldSelectOptionRepository.Update(selectOption);
            }
            else
            {
                selectOption = new CustomFieldSelectOption
                {
                    CustomFieldId = customField.Id,
                    Name = option.Name,
                    Color = option.Color
                };
                await _customFieldSelectOptionRepository.Create(selectOption);
            }
        }
    }

    public async Task<List<CustomField>> ListCustomFields(int projectId)
    {
        return await _customFieldRepository
            .Where(r => r.ProjectId == projectId)
            .Include(r => r.SelectOptions)
            .ToListAsync();
    }


    /*
     * Returns the following structured data
     * {
     *  [taskId]: {
     *      [customFieldId]: {
     *          value: 'text or number value or option id'
     *      }
     * }
     * }
     */
    public async Task<Dictionary<int, Dictionary<int, dynamic>>> ListTaskCustomFieldValuesByBoard(int boardId)
    {
        var responseDict = new Dictionary<int, Dictionary<int, dynamic>>();
        var tasksInBoard = await _taskService.ListBoardTasks(boardId);
        if (tasksInBoard.Count == 0)
        {
            return responseDict;
        }

        var taskIdsInBoard = tasksInBoard.Select(r => r.Id).ToList();
        var taskCustomFieldValues = await _taskCustomFieldValueRepository
            .Where(r => taskIdsInBoard.Contains(r.TaskId))
            .Include(r => r.CustomField)
            .ToListAsync();

        var allCustomFields = await _customFieldRepository
            .Where(r => r.ProjectId == tasksInBoard[0].ProjectId)
            .ToListAsync();

        foreach (var taskId in taskIdsInBoard)
        {
            // custom field computed value held by custom field id
            responseDict[taskId] = new Dictionary<int, dynamic>();

            foreach (var customField in allCustomFields)
            {
                // custom field value held by custom field id
                responseDict[taskId][customField.Id] = null;

                foreach (var customFieldValue in taskCustomFieldValues)
                {
                    if (customFieldValue.CustomFieldId == customField.Id && customFieldValue.TaskId == taskId)
                    {
                        dynamic computedValue = null;
                        if (customField.FieldType == CustomFieldType.Text)
                        {
                            computedValue = customFieldValue.Value;
                        }

                        if (customField.FieldType == CustomFieldType.Date)
                        {
                            computedValue = customFieldValue.Value;
                        }

                        if (customField.FieldType == CustomFieldType.Bool)
                        {
                            computedValue = bool.Parse(customFieldValue.Value);
                        }

                        if (customField.FieldType == CustomFieldType.Number)
                        {
                            if (string.IsNullOrEmpty(customFieldValue.Value))
                            {
                                computedValue = 0;
                            }
                            else
                            {
                                computedValue = Int32.Parse(customFieldValue.Value);
                            }
                        }
                        else if (customField.FieldType == CustomFieldType.SingleSelect)
                        {
                            if (string.IsNullOrEmpty(customFieldValue.Value) || customFieldValue.Value == "undefined")
                            {
                                computedValue = 0;
                            }
                            else
                            {
                                computedValue = Int32.Parse(customFieldValue.Value);
                            }
                        }
                        else if (customField.FieldType == CustomFieldType.MultiSelect)
                        {
                            if (string.IsNullOrEmpty(customFieldValue.Value))
                            {
                                computedValue = Array.Empty<int>();
                            }
                            else
                            {
                                var selectedOptionIds = customFieldValue.Value.Split(",").Select(Int32.Parse);
                                computedValue = selectedOptionIds;
                            }
                        }

                        responseDict[taskId][customField.Id] = computedValue;
                    }
                }
            }
        }

        return responseDict;
    }

    public async Task CreateCustomFieldTaskValue(CreateCustomFieldTaskValueInput input)
    {
        var taskCustomFieldValue = await _taskCustomFieldValueRepository
            .Where(r => r.TaskId == input.TaskId && r.CustomFieldId == input.CustomFieldId)
            .SingleOrDefaultAsync();

        if (taskCustomFieldValue != null)
        {
            // TODO find a way to cast properly here
            taskCustomFieldValue.Value = input.Value;
            await _taskCustomFieldValueRepository.Update(taskCustomFieldValue);
            return;
        }

        taskCustomFieldValue = new TaskCustomFieldValue
        {
            TaskId = input.TaskId,
            CustomFieldId = input.CustomFieldId,
            // TODO find a way to cast properly here
            Value = input.Value
        };
        await _taskCustomFieldValueRepository.Create(taskCustomFieldValue);
    }
}