using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkCleverSolution.Migrations
{
    public partial class BoardViewExtendWithUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Config",
                table: "BoardsViews",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BoardsViews",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BoardsViews_UserId",
                table: "BoardsViews",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardsViews_Users_UserId",
                table: "BoardsViews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardsViews_Users_UserId",
                table: "BoardsViews");

            migrationBuilder.DropIndex(
                name: "IX_BoardsViews_UserId",
                table: "BoardsViews");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BoardsViews");

            migrationBuilder.AlterColumn<string>(
                name: "Config",
                table: "BoardsViews",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
