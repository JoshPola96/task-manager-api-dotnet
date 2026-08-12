# TaskManagerAPI — .NET 8 Web API

> **Scope** · Timeboxed technical assessment · See [Scope & tradeoffs](#scope--tradeoffs)

> [!NOTE]
> **Built 2025. The ecosystem has moved since.**
> Dependencies here are unpinned, so a clean `pip install` today resolves to
> versions that did not exist when this was written and the .NET, EF Core and xUnit stack has moved on since. Expect install or
> runtime breakage on a fresh environment. What is on offer is the engineering
> approach and the decisions behind it, not a guaranteed-green build.
> Happy to bring it current if that would be useful — just ask.

Task and user management API with JWT authentication, role-based access, EF Core persistence, unit tests and a Docker build. Written against a five-part brief: API, database design, a debugging exercise, containerisation, and test coverage.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![EF Core](https://img.shields.io/badge/EF%20Core-in--memory-512BD4)
![Tests](https://img.shields.io/badge/tests-xUnit%20%2B%20Moq-brightgreen)
![Docker](https://img.shields.io/badge/docker-ready-2496ED)
![License](https://img.shields.io/badge/license-MIT-green)

---

## 1. Setup & run

**Stack:** .NET 8 Web API · Entity Framework Core (in-memory) · JWT auth · Swagger UI · Docker · xUnit + Moq

```bash
git clone https://github.com/JoshPola96/task-manager-api-dotnet.git
cd task-manager-api-dotnet/TaskManagerAPI
dotnet run
```

Swagger UI: <http://localhost:7296/swagger/index.html>

No database setup required — EF Core runs in-memory and seeds on start, so the API is usable the moment `dotnet run` returns.

---

## 2. Database design

* Schema and seed data: `schema.sql`
* EF Core models and migrations: `TaskManagerAPI/Migrations/`
* ER diagram: [`ER_diagram.png`](TaskManagerAPI/ER_diagram.png)

**Tables:** `Users`, `Tasks`, `TaskComments`

```sql
-- All tasks assigned to a user
SELECT * FROM Tasks WHERE AssignedUserId = 1;

-- All comments on a task
SELECT * FROM TaskComments WHERE TaskItemId = 2;
```

---

## 3. Debugging exercise

The brief supplied a broken service and asked for a corrected version.

**Given:**

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

Three defects: the methods are declared synchronous but return awaitables without `async`/`await`; the return types collide with `System.Threading.Task`; and `GetAllTasks` returns `Task<List<T>>` where `List<T>` is declared.

**Corrected:**

```csharp
public async Task<TaskEntity?> GetTask(int id)
{
    try
    {
        return await _dbContext.Set<TaskEntity>()
            .FirstOrDefaultAsync(t => t.Id == id);
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
```

The entity is renamed `TaskEntity` to remove the ambiguity with `System.Threading.Tasks.Task`, and the nullable return type makes "not found" part of the contract rather than a surprise.

---

## 4. Docker

```bash
docker build -t taskmanagerapi .
docker run -p 8080:8080 taskmanagerapi
```

Swagger: <http://localhost:8080/swagger/index.html>

Screenshots: `dockerrun.png`, `dockerswagger.png`, `renderswagger.png`. Previously deployed to Render's free tier — that instance may be spun down.

---

## 5. Tests

```bash
cd TaskManagerAPI.Tests
dotnet test
```

xUnit with Moq, covering the controller and service layer.

---

## Scope & tradeoffs

A timeboxed assessment, written alongside full-time work. What that bought, and what it cost:

**In scope, and complete**

* JWT authentication with role-based endpoint protection
* Full CRUD over tasks, users and comments
* xUnit + Moq coverage on the service and controller layer
* Dockerfile that builds and runs clean
* All five parts of the brief answered

**Deliberately out of scope**

* **Persistence is EF Core in-memory.** A real deployment needs Postgres or SQL Server plus migrations. The `DbContext` is abstracted behind the provider registration in `Program.cs`, so the swap is a one-line change and a connection string.
* **The JWT signing key is hardcoded** in `appsettings.json`. That is deliberate for a reviewable demo — the API works on `dotnet run` with no secret setup. In production it belongs in user-secrets, environment variables or a key vault, and it must be long enough to satisfy HMAC-SHA256 properly.
* **Access tokens are long-lived and there are no refresh tokens.** Wrong for production, convenient for a reviewer poking at Swagger.
* No rate limiting, structured logging, or health check endpoints.

**What I'd change with more time**

* Move validation out of the controllers into FluentValidation, so the rules are testable in isolation.
* Integration tests against a containerised Postgres via Testcontainers, rather than mocking the context.
* Replace the broad `catch (Exception)` wrappers with a global exception-handling middleware returning RFC 7807 problem details.

---

## License

MIT — see [LICENSE](LICENSE).
