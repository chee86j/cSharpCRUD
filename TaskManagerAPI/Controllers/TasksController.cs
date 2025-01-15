using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;
        private readonly ILogger<TasksController> _logger;

        public TasksController(
            AppDbContext context,
            UserManager<User> userManager,
            ILogger<TasksController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Fetching tasks for user {UserId}", userId);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} tasks for user {UserId}", tasks.Count, userId);
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask([FromBody] TaskCreateDto taskDto)
        {
            try
            {
                // Log all claims for debugging
                var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
                _logger.LogInformation("User claims: {@Claims}", claims);

                // Try to get email from different claim types
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value 
                    ?? User.FindFirst("sub")?.Value 
                    ?? User.FindFirst("email")?.Value;

                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogError("No email claim found in token");
                    return BadRequest(new { message = "User email not found in token" });
                }

                _logger.LogInformation("Looking up user with email: {Email}", userEmail);
                var user = await _userManager.FindByEmailAsync(userEmail);

                if (user == null)
                {
                    _logger.LogError("User not found for email: {Email}", userEmail);
                    return BadRequest(new { message = "Error creating task", error = "User not found" });
                }

                var task = new TaskItem
                {
                    Title = taskDto.Title,
                    Description = taskDto.Description,
                    Category = taskDto.Category,
                    UserId = user.Id,
                    User = user,
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Creating task for user: {UserId} with title: {Title}", user.Id, task.Title);
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, new { message = "Error creating task", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItem task)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingTask = await _context.Tasks.FindAsync(id);

            if (existingTask == null)
                return NotFound();

            if (existingTask.UserId != userId)
                return Unauthorized();

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

            if (task == null)
                return NotFound();

            if (task.UserId != userId)
                return Unauthorized();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class TaskCreateDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
    }
}
