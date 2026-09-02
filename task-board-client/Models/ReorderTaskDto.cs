namespace task_board_client.Models;

public class ReorderTaskDto
{
    public int TaskId { get; set; }
    public int TargetColumnId { get; set; }
    public int NewOrder { get; set; }
}
