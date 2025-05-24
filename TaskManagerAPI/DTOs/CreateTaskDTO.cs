namespace TaskManagerAPI.DTOs
{
    public class CreateTaskDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? AssignedUserId { get; set; } 
        public string Status { get; set; } = "Assigned";
    }
}