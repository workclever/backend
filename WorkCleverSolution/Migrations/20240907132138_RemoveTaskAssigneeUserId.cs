using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkCleverSolution.Migrations
{
    public partial class RemoveTaskAssigneeUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssigneeUserId",
                table: "TaskItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssigneeUserId",
                table: "TaskItems",
                type: "INTEGER",
                nullable: true);
        }
    }
}
