using System.ComponentModel.DataAnnotations;

namespace task_board_api.Models;

public class CreateSubtaskDto
{
    [Required(ErrorMessage = "El título es obligatorio")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
}

public class UpdateSubtaskDto
{
    [MaxLength(200)]
    public string? Title { get; set; }

    public bool? IsCompleted { get; set; }
}

public class CreateCommentDto
{
    [Required(ErrorMessage = "El autor es obligatorio")]
    [MaxLength(100)]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contenido es obligatorio")]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}
