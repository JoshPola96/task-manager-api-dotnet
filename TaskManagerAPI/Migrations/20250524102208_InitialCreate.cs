using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    AssignedUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TaskComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Comment = table.Column<string>(type: "TEXT", nullable: false),
                    TaskItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskComments_Tasks_TaskItemId",
                        column: x => x.TaskItemId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "admin", 0, "admin" },
                    { 2, "user", 1, "user" },
                    { 3, "newuser", 1, "newuser" }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "AssignedUserId", "Description", "Status", "Title" },
                values: new object[,]
                {
                    { 1, 2, "Test Description", "Completed", "Sample Task" },
                    { 2, 1, "Important", "Assigned", "Do something important" },
                    { 3, 3, "Arts", "Assigned", "Write a poem" },
                    { 4, 2, "CRUD", "Assigned", "Test API" }
                });

            migrationBuilder.InsertData(
                table: "TaskComments",
                columns: new[] { "Id", "Comment", "TaskItemId", "UserId" },
                values: new object[,]
                {
                    { 1, "Looks alright!", 1, 1 },
                    { 2, "Needs more detail.", 2, 1 },
                    { 3, "Starstruck", 3, 2 },
                    { 4, "Go through all the endpoints", 4, 1 },
                    { 5, "Check the imports", 4, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_TaskItemId",
                table: "TaskComments",
                column: "TaskItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_UserId",
                table: "TaskComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedUserId",
                table: "Tasks",
                column: "AssignedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskComments");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
