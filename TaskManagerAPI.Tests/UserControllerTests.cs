using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Controllers;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;
using Xunit;

public class UserControllerTests : IDisposable
{
    private readonly UserController _controller;
    private readonly TaskDbContext _context;

    public UserControllerTests()
    {
        // Setup in-memory EF Core DB for tests with unique database name
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TaskDbContext(options);

        // Ensure database is created and seed test data
        _context.Database.EnsureCreated();
        SeedTestData();

        _controller = new UserController(_context);
    }

    private void SeedTestData()
    {
        if (!_context.Users.Any())
        {
            _context.Users.AddRange(new List<User>
            {
                new User { Id = 1, Username = "admin", Password = "adminpass", Role = UserRole.Admin },
                new User { Id = 2, Username = "user1", Password = "userpass", Role = UserRole.User },
            });
            _context.SaveChanges();
        }
    }

    [Fact]
    public async Task GetAll_ReturnsAllUsers()
    {
        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task Get_ReturnsUser_WhenUserExists()
    {
        // Act
        var result = await _controller.Get(1);

        // Assert
        Assert.NotNull(result.Value);
        var userDto = Assert.IsType<UserDTO>(result.Value);
        Assert.Equal(1, userDto.Id);
        Assert.Equal("admin", userDto.Username);
        Assert.Equal(UserRole.Admin, userDto.Role);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Act
        var result = await _controller.Get(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedUser_WhenUsernameIsUnique()
    {
        // Arrange
        var dto = new CreateUserDTO { Username = "unique_user_123", Password = "pass123", Role = UserRole.User };


        // Act
        var result = await _controller.Create(dto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdUser = Assert.IsType<UserDTO>(createdAtActionResult.Value);
        Assert.Equal(dto.Username, createdUser.Username);
        Assert.Equal(dto.Role, createdUser.Role);

        // Verify user was actually created in database
        var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == "newuser");
        Assert.NotNull(userInDb);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenUsernameExists()
    {
        // Arrange
        var dto = new CreateUserDTO { Username = "admin", Password = "any", Role = UserRole.Admin };

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Username already exists", badRequestResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenUserExists()
    {
        // Act
        var result = await _controller.Delete(2);

        // Assert
        Assert.IsType<NoContentResult>(result);

        // Verify user was actually deleted
        var deletedUser = await _context.Users.FindAsync(2);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAllTaskSummaries_ReturnsTaskSummaries()
    {
        // Arrange - Add some test tasks
        var user1 = await _context.Users.FirstAsync(u => u.Id == 1);
        var task = new TaskItem
        {
            Title = "Test Task",
            Description = "Description",
            AssignedUserId = user1.Id,
            Status = "In Progress"
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetAllTaskSummaries();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}