using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeeTaskTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Designation = table.Column<string>(type: "TEXT", nullable: false),
                    DateOfJoining = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                });

            migrationBuilder.CreateTable(
                name: "TaskItems",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItems", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_TaskItems_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeId", "DateOfJoining", "Designation", "FullName", "Password", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Manager", "manager1", "AQAAAAEAACcQAAAAEJz9k3v9+u5uT1r6zjvYJpQh5rJ8k1vQ==", "Manager" },
                    { 2, new DateTime(2025, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Developer", "employee1", "AQAAAAEAACcQAAAAEJz9k3v9+u5uT1r6zjvYJpQh5rJ8k1vQ==", "Employee" },
                    { 3, new DateTime(2025, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tester", "employee2", "AQAAAAEAACcQAAAAEJz9k3v9+u5uT1r6zjvYJpQh5rJ8k1vQ==", "Employee" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_EmployeeId",
                table: "TaskItems",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskItems");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
