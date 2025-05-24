# ✅ TaskManagerAPI (.NET Core Web API)

A simple backend project to manage tasks and users with authentication, developed using .NET Core, EF Core, and JWT. Includes API endpoints, basic role management, unit testing, and Docker deployment.

---

## 🧩 1. API Setup & Run

### 📦 Technologies:
- .NET 8 Web API
- Entity Framework Core (In-Memory DB)
- JWT Authentication
- Swagger UI
- Docker
- xUnit + Moq

### ▶️ Run Locally

```bash
git clone https://github.com/JoshPola96/TaskManagerAPI.git
cd TaskManagerAPI/TaskManagerAPI
dotnet run
````

Open: [http://localhost:7296/swagger/index.html](http://localhost:7296/swagger/index.html)

---

## 🗃️ 2. Database Design

### 📁 Location:

* Schema & seed setup: `schema.sql`
* EF Core models: `/Migrations/`
* ER Diagram: [`ER_diagram.png`](ER_diagram.png)

### 📌 Tables:

* Users
* Tasks
* TaskComments

### 📄 Sample SQL Queries:

```sql
-- Get all tasks assigned to a user
SELECT * FROM Tasks WHERE AssignedUserId = 1;

-- Get all comments on a task
SELECT * FROM TaskComments WHERE TaskItemId = 2;
```

---

## 🛠️ 3. Debugging Fixes

### ❌ Original Bug:

```csharp
public Task<Task> GetTask(int id)
{
    return _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
}

public List<Task> GetAllTasks()
{
    return _dbContext.Tasks.ToListAsync();
}
```

### ✅ Fixed Version:

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class TaskService
{
    private readonly DbContext _dbContext;

    public TaskService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TaskEntity?> GetTask(int id)
    {
        try
        {
            return await _dbContext.Set<TaskEntity>().FirstOrDefaultAsync(t => t.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error fetching task with id {id}", ex);
        }
    }

    public async Task<List<TaskEntity>> GetAllTasks()
    {
        try
        {
            return await _dbContext.Set<TaskEntity>().ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching all tasks", ex);
        }
    }
}
```

---

## 🐳 4. Docker Deployment (Bonus)

### 📁 Dockerfile Included:

* Found at root: `TaskManagerAPI/Dockerfile`

### ▶️ Build & Run:

```bash
docker build -t taskmanagerapi .
docker run -p 8080:8080 taskmanagerapi    
```

* Swagger: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

> ✅ Screenshots available: `dockerrun.png, dockerswagger.png, renderswagger.png`, 

### 🔗 Deployment (TBD):

* Render: [https://taskmanagerapi-4q0e.onrender.com/swagger/index.html](https://taskmanagerapi-4q0e.onrender.com/swagger/index.html)

---

## 🧪 5. Unit Testing

### ✔️ Test Framework:

* **xUnit** with **Moq**
* Located in: `/TaskManagerAPI.Tests`

### ▶️ Run Tests:

```bash
cd TaskManagerAPI.Tests
dotnet test
```

## 👋 Note

This project was completed alongside other professional responsibilities. While time-constrained, the key parts were fully implemented, tested, and documented. A video walkthrough was skipped due to ongoing project work at the office.
