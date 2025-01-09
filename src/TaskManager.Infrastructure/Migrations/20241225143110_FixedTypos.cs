using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedTypos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Employees_AssinedEmployeeId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "AssinedEmployeeId",
                table: "Tasks",
                newName: "AssignedEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_AssinedEmployeeId",
                table: "Tasks",
                newName: "IX_Tasks_AssignedEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Employees_AssignedEmployeeId",
                table: "Tasks",
                column: "AssignedEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Employees_AssignedEmployeeId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "AssignedEmployeeId",
                table: "Tasks",
                newName: "AssinedEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_AssignedEmployeeId",
                table: "Tasks",
                newName: "IX_Tasks_AssinedEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Employees_AssinedEmployeeId",
                table: "Tasks",
                column: "AssinedEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
