namespace task_board_api.Models;

public class MoveTaskDto
{
    public int TaskId { get; set; }
    public int TargetColumnId { get; set; }
}
