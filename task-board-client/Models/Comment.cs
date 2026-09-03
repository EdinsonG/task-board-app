namespace task_board_client.Models;

public class Comment
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateCommentDto
{
    public string Author { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
