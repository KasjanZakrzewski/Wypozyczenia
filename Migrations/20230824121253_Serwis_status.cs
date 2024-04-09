using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WypozyczeniaAPI.Migrations
{
    public partial class Serwis_status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Serwis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Serwis");
        }
    }
}
