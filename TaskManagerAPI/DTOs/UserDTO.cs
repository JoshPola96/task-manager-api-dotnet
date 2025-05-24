using TaskManagerAPI.Models;

namespace TaskManagerAPI.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}