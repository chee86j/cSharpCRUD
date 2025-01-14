using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using Microsoft.AspNetCore.Cors;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TasksController> _logger;

        public TasksController(AppDbContext context, ILogger<TasksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _context.Tasks
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask(TaskCreateDto taskDto)
        {
            _logger.LogInformation("CreateTask called with data: {@taskDto}", taskDto);

            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    _logger.LogWarning("Validation failed: {errors}", errors);
                    return BadRequest(new
                    {
                        message = "Validation failed",
                        errors = errors
                    });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User not authenticated");
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var task = new TaskItem
                {
                    Title = taskDto.Title,
                    Description = taskDto.Description,
                    Category = taskDto.Category,
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId,
                    User = await _context.Users.FindAsync(userId) ?? throw new Exception("User not found")
                };

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Task created successfully: {@task}", task);

                return Ok(new
                {
                    id = task.Id,
                    title = task.Title,
                    description = task.Description,
                    category = task.Category,
                    isCompleted = task.IsCompleted,
                    createdAt = task.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return BadRequest(new { message = "Error creating task", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItem task)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingTask = await _context.Tasks.FindAsync(id);

            if (existingTask == null || existingTask.UserId != userId)
                return NotFound();

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Category = task.Category;
            existingTask.IsCompleted = task.IsCompleted;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var task = await _context.Tasks.FindAsync(id);

            if (task == null || task.UserId != userId)
                return NotFound();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class TaskCreateDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = string.Empty;
    }
}
