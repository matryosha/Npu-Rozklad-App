using Microsoft.EntityFrameworkCore.Migrations;

namespace RozkladNpuBot.Persistence.Migrations
{
    public partial class AddQueryCounterToRozkladUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "IsDeleted",
                table: "Users",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "QueryCount",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "QueryCount",
                table: "Users");
        }
    }
}
