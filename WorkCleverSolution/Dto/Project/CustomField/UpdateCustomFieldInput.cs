using System.ComponentModel.DataAnnotations;

namespace WorkCleverSolution.Dto.Project.CustomField;

public class UpdateCustomFieldInput
{
    [Required] public int CustomFieldId { get; set; }
    [Required] public string FieldName { get; set; }
    [Required] public string FieldType { get; set; }
    public bool Enabled { get; set; }
    public bool ShowInTaskCard { get; set; }

    public List<CustomFieldOptionInput> SelectOptions { get; set; }
}