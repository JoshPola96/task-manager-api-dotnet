namespace TaskManagerAPI.DTOs
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? AssignedUserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<CommentDTO> Comments { get; set; } = new();
    }
}