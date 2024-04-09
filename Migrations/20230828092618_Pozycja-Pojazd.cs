using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WypozyczeniaAPI.Migrations
{
    public partial class PozycjaPojazd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pozycja_Wypozyczenie_WypozyczenieId",
                table: "Pozycja");

            migrationBuilder.AlterColumn<int>(
                name: "WypozyczenieId",
                table: "Pozycja",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PojazdId",
                table: "Pozycja",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pozycja_PojazdId",
                table: "Pozycja",
                column: "PojazdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pozycja_Pojazd_PojazdId",
                table: "Pozycja",
                column: "PojazdId",
                principalTable: "Pojazd",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pozycja_Wypozyczenie_WypozyczenieId",
                table: "Pozycja",
                column: "WypozyczenieId",
                principalTable: "Wypozyczenie",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pozycja_Pojazd_PojazdId",
                table: "Pozycja");

            migrationBuilder.DropForeignKey(
                name: "FK_Pozycja_Wypozyczenie_WypozyczenieId",
                table: "Pozycja");

            migrationBuilder.DropIndex(
                name: "IX_Pozycja_PojazdId",
                table: "Pozycja");

            migrationBuilder.DropColumn(
                name: "PojazdId",
                table: "Pozycja");

            migrationBuilder.AlterColumn<int>(
                name: "WypozyczenieId",
                table: "Pozycja",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pozycja_Wypozyczenie_WypozyczenieId",
                table: "Pozycja",
                column: "WypozyczenieId",
                principalTable: "Wypozyczenie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
