using Microsoft.EntityFrameworkCore.Migrations;

namespace RozkladNpuBot.Persistence.Migrations
{
    public partial class GroupsUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FacultyShortName",
                table: "Group",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacultyShortName",
                table: "Group");
        }
    }
}
