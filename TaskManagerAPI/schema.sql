CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "Users" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
    "Username" TEXT NOT NULL,
    "Password" TEXT NOT NULL,
    "Role" INTEGER NOT NULL
);

CREATE TABLE "Tasks" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tasks" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "AssignedUserId" INTEGER NULL,
    "Status" TEXT NOT NULL,
    CONSTRAINT "FK_Tasks_Users_AssignedUserId" FOREIGN KEY ("AssignedUserId") REFERENCES "Users" ("Id") ON DELETE SET NULL
);

CREATE TABLE "TaskComments" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_TaskComments" PRIMARY KEY AUTOINCREMENT,
    "Comment" TEXT NOT NULL,
    "TaskItemId" INTEGER NOT NULL,
    "UserId" INTEGER NULL,
    CONSTRAINT "FK_TaskComments_Tasks_TaskItemId" FOREIGN KEY ("TaskItemId") REFERENCES "Tasks" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TaskComments_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE SET NULL
);

INSERT INTO "Users" ("Id", "Password", "Role", "Username")
VALUES (1, 'admin', 0, 'admin');
SELECT changes();

INSERT INTO "Users" ("Id", "Password", "Role", "Username")
VALUES (2, 'user', 1, 'user');
SELECT changes();

INSERT INTO "Users" ("Id", "Password", "Role", "Username")
VALUES (3, 'newuser', 1, 'newuser');
SELECT changes();


INSERT INTO "Tasks" ("Id", "AssignedUserId", "Description", "Status", "Title")
VALUES (1, 2, 'Test Description', 'Completed', 'Sample Task');
SELECT changes();

INSERT INTO "Tasks" ("Id", "AssignedUserId", "Description", "Status", "Title")
VALUES (2, 1, 'Important', 'Assigned', 'Do something important');
SELECT changes();

INSERT INTO "Tasks" ("Id", "AssignedUserId", "Description", "Status", "Title")
VALUES (3, 3, 'Arts', 'Assigned', 'Write a poem');
SELECT changes();

INSERT INTO "Tasks" ("Id", "AssignedUserId", "Description", "Status", "Title")
VALUES (4, 2, 'CRUD', 'Assigned', 'Test API');
SELECT changes();


INSERT INTO "TaskComments" ("Id", "Comment", "TaskItemId", "UserId")
VALUES (1, 'Looks alright!', 1, 1);
SELECT changes();

INSERT INTO "TaskComments" ("Id", "Comment", "TaskItemId", "UserId")
VALUES (2, 'Needs more detail.', 2, 1);
SELECT changes();

INSERT INTO "TaskComments" ("Id", "Comment", "TaskItemId", "UserId")
VALUES (3, 'Starstruck', 3, 2);
SELECT changes();

INSERT INTO "TaskComments" ("Id", "Comment", "TaskItemId", "UserId")
VALUES (4, 'Go through all the endpoints', 4, 1);
SELECT changes();

INSERT INTO "TaskComments" ("Id", "Comment", "TaskItemId", "UserId")
VALUES (5, 'Check the imports', 4, 2);
SELECT changes();


CREATE INDEX "IX_TaskComments_TaskItemId" ON "TaskComments" ("TaskItemId");

CREATE INDEX "IX_TaskComments_UserId" ON "TaskComments" ("UserId");

CREATE INDEX "IX_Tasks_AssignedUserId" ON "Tasks" ("AssignedUserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250524102208_InitialCreate', '9.0.5');

COMMIT;

