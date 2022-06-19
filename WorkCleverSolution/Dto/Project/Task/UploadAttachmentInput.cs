using System.ComponentModel.DataAnnotations;

namespace WorkCleverSolution.Dto.Project.Task;

public class UploadAttachmentInput
{
    [Required]
    public IFormFile file { get; set; }
}