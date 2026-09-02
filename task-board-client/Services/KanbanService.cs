using System.Net.Http.Json;
using task_board_client.Models;

namespace task_board_client.Services;

public class KanbanService
{
    private readonly HttpClient _http;

    public KanbanService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ColumnItem>?> GetBoardAsync()
    {
        return await _http.GetFromJsonAsync<List<ColumnItem>>("api/kanban");
    }

    public async Task<HttpResponseMessage> CreateTaskAsync(TaskItem task)
    {
        return await _http.PostAsJsonAsync("api/kanban/task", task);
    }

    public async Task<HttpResponseMessage> UpdateTaskAsync(int taskId, UpdateTaskDto dto)
    {
        return await _http.PutAsJsonAsync($"api/kanban/task/{taskId}", dto);
    }

    public async Task<HttpResponseMessage> DeleteTaskAsync(int taskId)
    {
        return await _http.DeleteAsync($"api/kanban/task/{taskId}");
    }

    public async Task<HttpResponseMessage> MoveTaskAsync(MoveTaskDto dto)
    {
        return await _http.PostAsJsonAsync("api/kanban/move-task", dto);
    }

    public async Task<HttpResponseMessage> ReorderTaskAsync(ReorderTaskDto dto)
    {
        return await _http.PostAsJsonAsync("api/kanban/reorder-task", dto);
    }

    public async Task<HttpResponseMessage> CreateColumnAsync(CreateColumnDto dto)
    {
        return await _http.PostAsJsonAsync("api/kanban/column", dto);
    }

    public async Task<HttpResponseMessage> UpdateColumnAsync(int columnId, UpdateColumnDto dto)
    {
        return await _http.PutAsJsonAsync($"api/kanban/column/{columnId}", dto);
    }

    public async Task<HttpResponseMessage> DeleteColumnAsync(int columnId)
    {
        return await _http.DeleteAsync($"api/kanban/column/{columnId}");
    }
}
