using Microsoft.AspNetCore.Mvc;
using WorkCleverSolution.Attributes;
using WorkCleverSolution.Attributes.Authorization;
using WorkCleverSolution.Attributes.Validation;
using WorkCleverSolution.Dto.Project.CustomField;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Controllers;

[Route("Api/CustomField/[action]")]
public class CustomFieldController : BaseApiController
{
    public CustomFieldController(IServices services) : base(services)
    {
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> CreateCustomField([FromBody] CreateCustomFieldInput input)
    {
        await Services.CustomFieldService().CreateCustomField(input);
        return Wrap();
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> UpdateCustomField([FromBody] UpdateCustomFieldInput input)
    {
        await Services.CustomFieldService().UpdateCustomField(input);
        return Wrap();
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListCustomFields([ValidProjectId] int projectId)
    {
        return Wrap(await Services.CustomFieldService().ListCustomFields(projectId));
    }

    [HttpDelete]
    [JwtAuthorize]
    public async Task<ServiceResult> DeleteCustomField([ValidProjectId] [ManageableProjectId] int projectId,
        int customFieldId)
    {
        await Services.CustomFieldService().DeleteCustomField(projectId, customFieldId);
        return Wrap();
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListTaskCustomFieldValuesByBoard([ValidBoardId] int boardId)
    {
        return Wrap(await Services.CustomFieldService().ListTaskCustomFieldValuesByBoard(boardId));
    }
    
    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> CreateCustomFieldTaskValue([FromBody] CreateCustomFieldTaskValueInput input)
    {
        await Services.CustomFieldService().CreateCustomFieldTaskValue(input);
        return Wrap();
    }
}