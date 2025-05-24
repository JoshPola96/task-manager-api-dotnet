using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TaskController : ControllerBase
    {
        private readonly TaskDbContext _context;
        public TaskController(TaskDbContext context) => _context = context;

        // --- Task CRUD ---

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IEnumerable<TaskDTO>> GetAll()
            => await _context.Tasks
                .Include(t => t.Comments)
                .Select(t => new TaskDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedUserId = t.AssignedUserId,
                    Status = t.Status,
                    Comments = t.Comments.Select(c => new CommentDTO { Comment = c.Comment, Id = c.Id }).ToList()
                }).ToListAsync();

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<TaskDTO>> Get(int id)
        {
            var t = await _context.Tasks
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (t == null) return NotFound();
            return new TaskDTO
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                AssignedUserId = t.AssignedUserId,
                Status = t.Status,
                Comments = t.Comments.Select(c => new CommentDTO { Comment = c.Comment, Id = c.Id }).ToList()
            };
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<TaskDTO>> Create(CreateTaskDTO dto)
        {
            // Validate assigned user exists
            if (!await _context.Users.AnyAsync(u => u.Id == dto.AssignedUserId))
                return BadRequest("Assigned user does not exist");

            var t = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                AssignedUserId = dto.AssignedUserId,
                Status = dto.Status
            };
            _context.Tasks.Add(t);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = t.Id },
               new TaskDTO
               {
                   Id = t.Id,
                   Title = t.Title,
                   Description = t.Description,
                   AssignedUserId = t.AssignedUserId,
                   Status = t.Status,
                   Comments = new List<CommentDTO>() // Empty comments list for new task
               });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Update(int id, CreateTaskDTO dto)
        {
            var t = await _context.Tasks.FindAsync(id);
            if (t == null) return NotFound();

            // Validate assigned user exists
            if (!await _context.Users.AnyAsync(u => u.Id == dto.AssignedUserId))
                return BadRequest("Assigned user does not exist");

            t.Title = dto.Title;
            t.Description = dto.Description;
            t.AssignedUserId = dto.AssignedUserId;
            t.Status = dto.Status;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _context.Tasks.FindAsync(id);
            if (t == null) return NotFound();
            _context.Tasks.Remove(t);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // --- Comment Endpoints ---

        [HttpPost("{taskId}/comments")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<CommentDTO>> AddComment(int taskId, CreateCommentDTO dto)
        {
            if (!await _context.Tasks.AnyAsync(t => t.Id == taskId))
                return NotFound("Task not found");

            var c = new TaskComment
            {
                Comment = dto.Comment,
                TaskItemId = taskId
            };
            _context.TaskComments.Add(c);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComment), new { taskId = taskId, commentId = c.Id },
               new CommentDTO
               {
                   Id = c.Id,
                   Comment = c.Comment
               });
        }

        [HttpGet("/comments/{commentId}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<CommentDTO>> GetComment(int commentId)
        {
            var c = await _context.TaskComments
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (c == null) return NotFound();

            return new CommentDTO { Id = c.Id, Comment = c.Comment };
        }

        [HttpPut("comments/{commentId}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> EditComment(int commentId, CreateCommentDTO dto)
        {
            var c = await _context.TaskComments.FindAsync(commentId);
            if (c == null) return NotFound();

            c.Comment = dto.Comment;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("/comments/{commentId}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var c = await _context.TaskComments.FindAsync(commentId);
            if (c == null) return NotFound();

            _context.TaskComments.Remove(c);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}