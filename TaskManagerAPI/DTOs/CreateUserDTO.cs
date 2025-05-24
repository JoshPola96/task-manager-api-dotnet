using TaskManagerAPI.Models;

namespace TaskManagerAPI.DTOs
{
    public class CreateUserDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}