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

    [HttpGet("task/{taskId}/subtasks")]
    public async Task<ActionResult<List<Subtask>>> GetSubtasks(int taskId)
    {
        try
        {
            var subtasks = await _db.Subtasks
                .Where(s => s.TaskId == taskId)
                .OrderBy(s => s.Order)
                .ToListAsync();

            return Ok(subtasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener subtareas de la tarea {TaskId}", taskId);
            return StatusCode(500, new { message = "Error al obtener subtareas" });
        }
    }

    [HttpPost("task/{taskId}/subtask")]
    public async Task<ActionResult<Subtask>> CreateSubtask(int taskId, [FromBody] CreateSubtaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var taskExists = await _db.Tasks.AnyAsync(t => t.Id == taskId);
            if (!taskExists)
                return NotFound(new { message = $"Tarea con ID {taskId} no encontrada" });

            var maxOrder = await _db.Subtasks.Where(s => s.TaskId == taskId).MaxAsync(s => (int?)s.Order) ?? 0;

            var subtask = new Subtask
            {
                TaskId = taskId,
                Title = dto.Title,
                IsCompleted = false,
                Order = maxOrder + 1
            };

            _db.Subtasks.Add(subtask);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Subtarea creada: {SubtaskId} - {Title} en tarea {TaskId}", subtask.Id, subtask.Title, taskId);
            return Ok(subtask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear subtarea en tarea {TaskId}", taskId);
            return StatusCode(500, new { message = "Error al crear subtarea" });
        }
    }

    [HttpPut("subtask/{id}")]
    public async Task<IActionResult> UpdateSubtask(int id, [FromBody] UpdateSubtaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var subtask = await _db.Subtasks.FindAsync(id);
            if (subtask == null)
                return NotFound(new { message = $"Subtarea con ID {id} no encontrada" });

            if (dto.Title != null)
                subtask.Title = dto.Title;

            if (dto.IsCompleted.HasValue)
                subtask.IsCompleted = dto.IsCompleted.Value;

            await _db.SaveChangesAsync();

            _logger.LogInformation("Subtarea actualizada: {SubtaskId}", id);
            return Ok(subtask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar subtarea {SubtaskId}", id);
            return StatusCode(500, new { message = "Error al actualizar subtarea" });
        }
    }

    [HttpDelete("subtask/{id}")]
    public async Task<IActionResult> DeleteSubtask(int id)
    {
        try
        {
            var subtask = await _db.Subtasks.FindAsync(id);
            if (subtask == null)
                return NotFound(new { message = $"Subtarea con ID {id} no encontrada" });

            _db.Subtasks.Remove(subtask);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Subtarea eliminada: {SubtaskId}", id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar subtarea {SubtaskId}", id);
            return StatusCode(500, new { message = "Error al eliminar subtarea" });
        }
    }

    [HttpGet("task/{taskId}/comments")]
    public async Task<ActionResult<List<Comment>>> GetComments(int taskId)
    {
        try
        {
            var comments = await _db.Comments
                .Where(c => c.TaskId == taskId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener comentarios de la tarea {TaskId}", taskId);
            return StatusCode(500, new { message = "Error al obtener comentarios" });
        }
    }

    [HttpPost("task/{taskId}/comment")]
    public async Task<ActionResult<Comment>> CreateComment(int taskId, [FromBody] CreateCommentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var taskExists = await _db.Tasks.AnyAsync(t => t.Id == taskId);
            if (!taskExists)
                return NotFound(new { message = $"Tarea con ID {taskId} no encontrada" });

            var comment = new Comment
            {
                TaskId = taskId,
                Author = dto.Author,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Comentario creado: {CommentId} en tarea {TaskId}", comment.Id, taskId);
            return Ok(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear comentario en tarea {TaskId}", taskId);
            return StatusCode(500, new { message = "Error al crear comentario" });
        }
    }

    [HttpDelete("comment/{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        try
        {
            var comment = await _db.Comments.FindAsync(id);
            if (comment == null)
                return NotFound(new { message = $"Comentario con ID {id} no encontrado" });

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Comentario eliminado: {CommentId}", id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar comentario {CommentId}", id);
            return StatusCode(500, new { message = "Error al eliminar comentario" });
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

            var newOrder = await _db.Tasks.CountAsync(t => t.ColumnId == dto.TargetColumnId) + 1;

            task.ColumnId = dto.TargetColumnId;
            task.Order = newOrder;

            await _db.SaveChangesAsync();

            _logger.LogInformation("Tarea {TaskId} movida a columna {ColumnId} (orden {Order})", dto.TaskId, dto.TargetColumnId, newOrder);
            return Ok(new { task.Id, task.ColumnId, task.Order });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al mover la tarea {TaskId}", dto.TaskId);
            return StatusCode(500, new { message = "Error al mover la tarea" });
        }
    }

    [HttpPost("reorder-task")]
    public async Task<IActionResult> ReorderTask([FromBody] ReorderTaskDto dto)
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

            var tasksInColumn = await _db.Tasks
                .Where(t => t.ColumnId == dto.TargetColumnId && t.Id != dto.TaskId)
                .OrderBy(t => t.Order)
                .ToListAsync();

            task.ColumnId = dto.TargetColumnId;
            tasksInColumn.Insert(Math.Min(dto.NewOrder, tasksInColumn.Count), task);

            for (int i = 0; i < tasksInColumn.Count; i++)
            {
                tasksInColumn[i].Order = i + 1;
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation("Tarea {TaskId} reordenada en columna {ColumnId} a posición {Order}", dto.TaskId, dto.TargetColumnId, task.Order);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reordenar la tarea {TaskId}", dto.TaskId);
            return StatusCode(500, new { message = "Error al reordenar la tarea" });
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
            task.DueDate = dto.DueDate;
            task.Labels = dto.Labels ?? string.Empty;

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

    [HttpPost("column")]
    public async Task<ActionResult<ColumnItem>> CreateColumn([FromBody] CreateColumnDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var maxOrder = await _db.Columns.MaxAsync(c => (int?)c.Order) ?? 0;

            var column = new ColumnItem
            {
                Name = dto.Name,
                Order = maxOrder + 1
            };

            _db.Columns.Add(column);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Columna creada: {ColumnId} - {Name}", column.Id, column.Name);
            return Ok(column);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la columna");
            return StatusCode(500, new { message = "Error al crear la columna" });
        }
    }

    [HttpPut("column/{id}")]
    public async Task<IActionResult> UpdateColumn(int id, [FromBody] UpdateColumnDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var column = await _db.Columns.FindAsync(id);
            if (column == null)
                return NotFound(new { message = $"Columna con ID {id} no encontrada" });

            column.Name = dto.Name;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Columna actualizada: {ColumnId} - {Name}", id, dto.Name);
            return Ok(column);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la columna {ColumnId}", id);
            return StatusCode(500, new { message = "Error al actualizar la columna" });
        }
    }

    [HttpDelete("column/{id}")]
    public async Task<IActionResult> DeleteColumn(int id)
    {
        try
        {
            var column = await _db.Columns.Include(c => c.Tasks).FirstOrDefaultAsync(c => c.Id == id);
            if (column == null)
                return NotFound(new { message = $"Columna con ID {id} no encontrada" });

            _db.Columns.Remove(column);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Columna eliminada: {ColumnId} - {Name}", id, column.Name);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la columna {ColumnId}", id);
            return StatusCode(500, new { message = "Error al eliminar la columna" });
        }
    }
}
