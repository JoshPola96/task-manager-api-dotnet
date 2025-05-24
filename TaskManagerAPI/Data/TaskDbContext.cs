using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<TaskItem> Tasks { get; set; } = null!;
        public DbSet<TaskComment> TaskComments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "admin", Role = UserRole.Admin },
                new User { Id = 2, Username = "user", Password = "user", Role = UserRole.User },
                new User { Id = 3, Username = "newuser", Password = "newuser", Role = UserRole.User }
            );

            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem { Id = 1, Title = "Sample Task", Description = "Test Description", AssignedUserId = 2, Status = "Completed" },
                new TaskItem { Id = 2, Title = "Do something important", Description = "Important", AssignedUserId = 1, Status = "Assigned" },
                new TaskItem { Id = 3, Title = "Write a poem", Description = "Arts", AssignedUserId = 3, Status = "Assigned" },
                new TaskItem { Id = 4, Title = "Test API", Description = "CRUD", AssignedUserId = 2, Status = "Assigned" }
            );

            modelBuilder.Entity<TaskComment>().HasData(
                new TaskComment { Id = 1, Comment = "Looks alright!", TaskItemId = 1, UserId = 1 },
                new TaskComment { Id = 2, Comment = "Needs more detail.", TaskItemId = 2, UserId = 1 },
                new TaskComment { Id = 3, Comment = "Starstruck", TaskItemId = 3, UserId = 2 },
                new TaskComment { Id = 4, Comment = "Go through all the endpoints", TaskItemId = 4, UserId = 1 },
                new TaskComment { Id = 5, Comment = "Check the imports", TaskItemId = 4, UserId = 2 }
            );

            // Configure cascade delete for Task -> TaskComments
            modelBuilder.Entity<TaskComment>()
                .HasOne(tc => tc.TaskItem)
                .WithMany(t => t.Comments)
                .HasForeignKey(tc => tc.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship for User -> TaskComments (SetNull instead of cascade to preserve comments)
            modelBuilder.Entity<TaskComment>()
                .HasOne(tc => tc.User)
                .WithMany()
                .HasForeignKey(tc => tc.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure relationship for User -> Tasks (SetNull to prevent accidental data loss)
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.AssignedUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }
    }
}