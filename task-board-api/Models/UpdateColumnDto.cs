using System.ComponentModel.DataAnnotations;

namespace task_board_api.Models;

public class UpdateColumnDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Name { get; set; } = string.Empty;
}
