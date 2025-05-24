using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly TaskDbContext _context;
        public UserController(TaskDbContext context) => _context = context;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<UserDTO>> GetAll()
            => await _context.Users
                .Select(u => new UserDTO { Id = u.Id, Username = u.Username, Role = u.Role })
                .ToListAsync();

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            var u = await _context.Users.FindAsync(id);
            if (u == null) return NotFound();
            return new UserDTO { Id = u.Id, Username = u.Username, Role = u.Role };
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> Create(CreateUserDTO dto)
        {
            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Username already exists");

            var user = new User { Username = dto.Username, Password = dto.Password, Role = dto.Role };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = user.Id },
               new UserDTO { Id = user.Id, Username = user.Username, Role = user.Role });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _context.Users.FindAsync(id);
            if (u == null) return NotFound();
            _context.Users.Remove(u);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("task-summaries")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllTaskSummaries()
        {
            var tasks = await _context.Tasks
                .Include(t => t.AssignedUser)
                .Include(t => t.Comments)
                .ToListAsync();

            var result = tasks.Select(task => new
            {
                TaskId = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Username = task.AssignedUser?.Username ?? "Unknown",
                Comments = task.Comments.Select(c => c.Comment).ToList()
            });

            return Ok(result);
        }
    }
}