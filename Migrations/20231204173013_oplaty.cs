using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WypozyczeniaAPI.Migrations
{
    public partial class oplaty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Oplata",
                table: "Wypozyczenie",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Oplata",
                table: "Wypozyczenie");
        }
    }
}
