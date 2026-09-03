using System.ComponentModel.DataAnnotations;

namespace task_board_api.Models;

public class Subtask
{
    public int Id { get; set; }

    [Required]
    public int TaskId { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public int Order { get; set; }
}
