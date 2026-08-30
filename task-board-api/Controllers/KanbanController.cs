using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task_board_api.Data;
using task_board_api.Models;

namespace task_board_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KanbanController : ControllerBase
{
    private readonly AppDbContext _db;

    public KanbanController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<ColumnItem>>> GetBoard()
    {
        return await _db.Columns
            .Include(c => c.Tasks)
            .OrderBy(c => c.Order)
            .ToListAsync();
    }

    [HttpPost("task")]
    public async Task<ActionResult<TaskItem>> CreateTask(TaskItem task)
    {
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return Ok(task);
    }

    [HttpPost("move-task")]
    public async Task<IActionResult> MoveTask([FromBody] MoveTaskDto dto)
    {
        var task = await _db.Tasks.FindAsync(dto.TaskId);
        if (task == null) return NotFound();

        task.ColumnId = dto.TargetColumnId;
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("task/{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();

        return Ok();
    }
}
