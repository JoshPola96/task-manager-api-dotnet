namespace TaskManagerAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? AssignedUserId { get; set; }
        public string Status { get; set; } = "Assigned";
        public User? AssignedUser { get; set; }
        public List<TaskComment> Comments { get; set; } = new();
    }
}