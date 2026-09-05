namespace task_board_client.Services;

public class BoardService
{
    public event Action? OnToggleFilters;
    public event Func<Task>? OnExport;
    public event Action? OnManageLabels;
    public event Action? OnNewBoard;
    public event Action<string>? OnViewChanged;

    public void ToggleFilters() => OnToggleFilters?.Invoke();
    public async Task Export() { if (OnExport != null) await OnExport.Invoke(); }
    public void ManageLabels() => OnManageLabels?.Invoke();
    public void NewBoard() => OnNewBoard?.Invoke();
    public void ChangeView(string view) => OnViewChanged?.Invoke(view);
}
