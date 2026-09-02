using System.ComponentModel.DataAnnotations;

namespace task_board_api.Models;

public class UpdateTaskDto
{
    [Required(ErrorMessage = "El título es obligatorio")]
    [MaxLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    [MaxLength(200)]
    public string Labels { get; set; } = string.Empty;
}
