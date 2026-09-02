using System.ComponentModel.DataAnnotations;

namespace task_board_api.Models;

public class ReorderTaskDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int TaskId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int TargetColumnId { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int NewOrder { get; set; }
}
