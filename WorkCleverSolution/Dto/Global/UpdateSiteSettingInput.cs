using System.ComponentModel.DataAnnotations;

namespace WorkCleverSolution.Dto.Global;

public class UpdateSiteSettingInput
{
    [Required] public string Property { get; set; }
    public string Value { get; set; }
}