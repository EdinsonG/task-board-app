using System.ComponentModel.DataAnnotations;

namespace task_board_api.Models;

public class MoveTaskDto
{
    [Required(ErrorMessage = "El ID de la tarea es obligatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de la tarea no es válido")]
    public int TaskId { get; set; }

    [Required(ErrorMessage = "La columna destino es obligatoria")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de la columna destino no es válido")]
    public int TargetColumnId { get; set; }
}
