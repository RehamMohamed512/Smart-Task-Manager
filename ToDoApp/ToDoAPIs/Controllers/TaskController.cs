using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo.Core.DTos;
using ToDo.Core.Models;
using ToDo.Infrastructure.Data;

namespace ToDo.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ToDoDbContext _context;

        public TaskController(ToDoDbContext context)
        {
            _context = context;
        }

        // Get all tasks for a specific user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTasksByUser(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { StatusCode = 404, Message = $"User with Id {userId} not found" });
                }

                var tasks = await _context.ToDoTasks
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                if (tasks == null || !tasks.Any())
                {
                    return NotFound(new { StatusCode = 404, Message = $"No tasks found for User {userId}" });
                }

                return Ok(new { StatusCode = 200, Message = "Success", Data = tasks });
            }
            catch (Exception ex)
            {
                return BadRequest(new { StatusCode = 400, Message = "Something went wrong", Error = ex.Message });
            }
        }

        // Create task for user
        [HttpPost("user/{userId}")]
        public async Task<IActionResult> Create(int userId, [FromBody] ToDoTaskDTos taskDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { StatusCode = 404, Message = $"User with Id {userId} not found" });
                }

                if (string.IsNullOrWhiteSpace(taskDto.Title) || string.IsNullOrWhiteSpace(taskDto.Description))
                {
                    return BadRequest(new { Message = "Title and Description are required." });
                }

                // mapping DTO → Model
                var task = new ToDoTask
                {
                    Title = taskDto.Title,
                    Description = taskDto.Description,
                    DueDate = taskDto.DueDate,
                    UserId = userId,
                    IsCompleted = false
                };

                _context.ToDoTasks.Add(task);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Task created successfully", Data = task });
            }
            catch (Exception ex)
            {
                return BadRequest(new { StatusCode = 400, Message = "Something went wrong", Error = ex.Message });
            }
        }

        // Update task (specific user)
        [HttpPut("{taskId}/user/{userId}")]
        public async Task<IActionResult> Update(int taskId, int userId, [FromBody] ToDoTaskDTos updatedTask)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { StatusCode = 404, Message = $"User with Id {userId} not found" });
                }

                var existingTask = await _context.ToDoTasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
                if (existingTask == null)
                {
                    return NotFound(new { StatusCode = 404, Message = $"Task {taskId} not found for User {userId}" });
                }

                existingTask.Title = updatedTask.Title;
                existingTask.Description = updatedTask.Description;
                //existingTask.IsCompleted = updatedTask.IsCompleted;
                existingTask.DueDate = updatedTask.DueDate;

                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 200, Message = "Task updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { StatusCode = 400, Message = "Something went wrong", Error = ex.Message });
            }
        }

        // Delete task (specific user)
        [HttpDelete("{taskId}/user/{userId}")]
        public async Task<IActionResult> Delete(int taskId, int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { StatusCode = 404, Message = $"User with Id {userId} not found" });
                }

                var task = await _context.ToDoTasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
                if (task == null)
                {
                    return NotFound(new { StatusCode = 404, Message = $"Task {taskId} not found for User {userId}" });
                }

                _context.ToDoTasks.Remove(task);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 200, Message = "Task deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { StatusCode = 400, Message = "Something went wrong", Error = ex.Message });
            }
        }

        [HttpPatch("{taskId}/toggle/user/{userId}")]
        public async Task<IActionResult> ToggleCompletion(int taskId, int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { StatusCode = 404, Message = $"User with Id {userId} not found" });
                }

                var task = await _context.ToDoTasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
                if (task == null)
                {
                    return NotFound(new { StatusCode = 404, Message = $"Task {taskId} not found for User {userId}" });
                }

                task.IsCompleted = !task.IsCompleted;
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 200, Message = "Task status updated", Data = task });
            }
            catch (Exception ex)
            {
                return BadRequest(new { StatusCode = 400, Message = "Something went wrong", Error = ex.Message });
            }
        }
    }
}