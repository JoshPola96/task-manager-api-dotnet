namespace TaskManagerAPI.DTOs
{
    public class TaskDetailsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public UserDTO AssignedUser { get; set; } = new();
        public List<CommentDTO> Comments { get; set; } = new();
    }
}




