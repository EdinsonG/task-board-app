using System.ComponentModel.DataAnnotations;

namespace task_board_api.Models;

public class Comment
{
    public int Id { get; set; }

    [Required]
    public int TaskId { get; set; }

    [Required(ErrorMessage = "El autor es obligatorio")]
    [MaxLength(100)]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contenido es obligatorio")]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
