using Microsoft.EntityFrameworkCore.Migrations;

namespace NpuRozklad.Persistence.EfMigrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RozkladUserWrappers",
                columns: table => new
                {
                    Guid = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    FacultyGroupsTypeIds = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RozkladUserWrappers", x => x.Guid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RozkladUserWrappers");
        }
    }
}
