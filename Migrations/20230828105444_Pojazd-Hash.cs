using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WypozyczeniaAPI.Migrations
{
    public partial class PojazdHash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Pojazd",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Pojazd");
        }
    }
}
