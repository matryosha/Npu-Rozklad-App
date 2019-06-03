using Microsoft.EntityFrameworkCore.Migrations;

namespace RozkladNpuBot.Persistence.Migrations
{
    public partial class UpdateRozkladUserGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Group_Users_RozkladUserGuid",
                table: "Group");

            migrationBuilder.DropIndex(
                name: "IX_Group_RozkladUserGuid",
                table: "Group");

            migrationBuilder.DropColumn(
                name: "RozkladUserGuid",
                table: "Group");

            migrationBuilder.AddColumn<string>(
                name: "GroupsAsJson",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupsAsJson",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "RozkladUserGuid",
                table: "Group",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Group_RozkladUserGuid",
                table: "Group",
                column: "RozkladUserGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Users_RozkladUserGuid",
                table: "Group",
                column: "RozkladUserGuid",
                principalTable: "Users",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
