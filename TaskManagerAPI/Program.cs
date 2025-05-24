using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using TaskManagerAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseInMemoryDatabase("TaskDb"));

/*
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite("Data Source=taskmanager.db"));
*/

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT:Key is missing from configuration.");

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers().AddJsonOptions(opts => { opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()) ;   }   );
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskManagerAPI", Version = "v1" });

    // 🔐 Add JWT Bearer security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // <-- lowercase "bearer"
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token only. Example: eyJhbGciOiJIUzI1NiIs..."
    });

    // 🔐 Apply to all endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
});
});

var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API v1");
    });

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
