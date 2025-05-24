using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly TaskDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(TaskDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDTO login)
        {
            // 1. Validate credentials
            var user = _context.Users
                .FirstOrDefault(u =>
                    u.Username == login.Username &&
                    u.Password == login.Password);

            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            // 2. Create JWT claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            // 3. Generate token
            var keyString = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT:Key is missing from configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            // 4. Return token
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
