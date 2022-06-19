using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.CustomField;

public class CreateCustomFieldTaskValueInput
{
    [ValidTaskId] [Required] public int TaskId { get; set; }
    [Required] public int CustomFieldId { get; set; }
    public string Value { get; set; }
}