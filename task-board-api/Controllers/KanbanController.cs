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
    private readonly ILogger<KanbanController> _logger;

    public KanbanController(AppDbContext db, ILogger<KanbanController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<ColumnItem>>> GetBoard()
    {
        try
        {
            var columns = await _db.Columns
                .Include(c => c.Tasks)
                .OrderBy(c => c.Order)
                .ToListAsync();

            return Ok(columns);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tablero");
            return StatusCode(500, new { message = "Error al obtener el tablero" });
        }
    }

    [HttpPost("task")]
    public async Task<ActionResult<TaskItem>> CreateTask([FromBody] TaskItem task)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var columnExists = await _db.Columns.AnyAsync(c => c.Id == task.ColumnId);
            if (!columnExists)
                return BadRequest(new { message = $"La columna con ID {task.ColumnId} no existe" });

            task.Order = await _db.Tasks.CountAsync(t => t.ColumnId == task.ColumnId) + 1;

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Tarea creada: {TaskId} - {Title}", task.Id, task.Title);
            return Ok(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la tarea");
            return StatusCode(500, new { message = "Error al crear la tarea" });
        }
    }

    [HttpPost("move-task")]
    public async Task<IActionResult> MoveTask([FromBody] MoveTaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var task = await _db.Tasks.FindAsync(dto.TaskId);
            if (task == null)
                return NotFound(new { message = $"Tarea con ID {dto.TaskId} no encontrada" });

            var columnExists = await _db.Columns.AnyAsync(c => c.Id == dto.TargetColumnId);
            if (!columnExists)
                return BadRequest(new { message = $"La columna con ID {dto.TargetColumnId} no existe" });

            task.ColumnId = dto.TargetColumnId;
            task.Order = await _db.Tasks.CountAsync(t => t.ColumnId == dto.TargetColumnId) + 1;

            await _db.SaveChangesAsync();

            _logger.LogInformation("Tarea {TaskId} movida a columna {ColumnId}", dto.TaskId, dto.TargetColumnId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al mover la tarea {TaskId}", dto.TaskId);
            return StatusCode(500, new { message = "Error al mover la tarea" });
        }
    }

    [HttpPut("task/{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null)
                return NotFound(new { message = $"Tarea con ID {id} no encontrada" });

            task.Title = dto.Title;
            task.Description = dto.Description;

            await _db.SaveChangesAsync();

            _logger.LogInformation("Tarea actualizada: {TaskId} - {Title}", id, dto.Title);
            return Ok(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la tarea {TaskId}", id);
            return StatusCode(500, new { message = "Error al actualizar la tarea" });
        }
    }

    [HttpDelete("task/{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null)
                return NotFound(new { message = $"Tarea con ID {id} no encontrada" });

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Tarea eliminada: {TaskId}", id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la tarea {TaskId}", id);
            return StatusCode(500, new { message = "Error al eliminar la tarea" });
        }
    }
}
