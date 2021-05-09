using Microsoft.EntityFrameworkCore.Migrations;

namespace MiBandNaramek.Migrations
{
    public partial class InitialCreate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Steps",
                table: "ActivityData",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Steps",
                table: "ActivityData");
        }
    }
}
