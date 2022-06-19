using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Authorization;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.CustomField;

public class CreateCustomFieldInput
{
    [Required]
    [ValidProjectId]
    [ManageableProjectId]
    public int ProjectId { get; set; }

    [Required] public string FieldName { get; set; }

    [Required] public string FieldType { get; set; }

    public bool Enabled { get; set; }
    public bool ShowInTaskCard { get; set; }
    public List<CustomFieldOptionInput> SelectOptions { get; set; }
}

// Shared for both update/create
public class CustomFieldOptionInput
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
}