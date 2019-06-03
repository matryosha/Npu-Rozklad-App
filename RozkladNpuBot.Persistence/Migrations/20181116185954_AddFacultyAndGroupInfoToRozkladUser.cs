using Microsoft.EntityFrameworkCore.Migrations;

namespace RozkladNpuBot.Persistence.Migrations
{
    public partial class AddFacultyAndGroupInfoToRozkladUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExternalGroupId",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FacultyShortName",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalGroupId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FacultyShortName",
                table: "Users");
        }
    }
}
