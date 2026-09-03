namespace task_board_client.Models;

public class Subtask
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int Order { get; set; }
}

public class CreateSubtaskDto
{
    public string Title { get; set; } = string.Empty;
}

public class UpdateSubtaskDto
{
    public string? Title { get; set; }
    public bool? IsCompleted { get; set; }
}
