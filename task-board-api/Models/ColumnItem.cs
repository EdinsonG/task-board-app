namespace task_board_api.Models;

public class ColumnItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<TaskItem> Tasks { get; set; } = new();
}
