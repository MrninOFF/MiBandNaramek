using Microsoft.EntityFrameworkCore.Migrations;

namespace MiBandNaramek.Migrations
{
    public partial class InitialCreate4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "AspNetUsers",
                type: "decimal(38,18)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Wight",
                table: "AspNetUsers",
                type: "decimal(38,18)",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Wight",
                table: "AspNetUsers");
        }
    }
}
