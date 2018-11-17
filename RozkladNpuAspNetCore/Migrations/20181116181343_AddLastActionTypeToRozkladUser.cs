using Microsoft.EntityFrameworkCore.Migrations;

namespace RozkladNpuAspNetCore.Migrations
{
    public partial class AddLastActionTypeToRozkladUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastAction",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAction",
                table: "Users");
        }
    }
}
