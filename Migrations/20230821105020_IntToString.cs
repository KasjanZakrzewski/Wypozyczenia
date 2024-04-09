using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WypozyczeniaAPI.Migrations
{
    public partial class IntToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rezerwacja_AspNetUsers_UzytkownikId1",
                table: "Rezerwacja");

            migrationBuilder.DropForeignKey(
                name: "FK_Wypozyczenie_AspNetUsers_UzytkownikId1",
                table: "Wypozyczenie");

            migrationBuilder.DropIndex(
                name: "IX_Wypozyczenie_UzytkownikId1",
                table: "Wypozyczenie");

            migrationBuilder.DropIndex(
                name: "IX_Rezerwacja_UzytkownikId1",
                table: "Rezerwacja");

            migrationBuilder.DropColumn(
                name: "UzytkownikId1",
                table: "Wypozyczenie");

            migrationBuilder.DropColumn(
                name: "UzytkownikId1",
                table: "Rezerwacja");

            migrationBuilder.AlterColumn<string>(
                name: "UzytkownikId",
                table: "Wypozyczenie",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UzytkownikId",
                table: "Rezerwacja",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Wypozyczenie_UzytkownikId",
                table: "Wypozyczenie",
                column: "UzytkownikId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezerwacja_UzytkownikId",
                table: "Rezerwacja",
                column: "UzytkownikId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rezerwacja_AspNetUsers_UzytkownikId",
                table: "Rezerwacja",
                column: "UzytkownikId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wypozyczenie_AspNetUsers_UzytkownikId",
                table: "Wypozyczenie",
                column: "UzytkownikId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rezerwacja_AspNetUsers_UzytkownikId",
                table: "Rezerwacja");

            migrationBuilder.DropForeignKey(
                name: "FK_Wypozyczenie_AspNetUsers_UzytkownikId",
                table: "Wypozyczenie");

            migrationBuilder.DropIndex(
                name: "IX_Wypozyczenie_UzytkownikId",
                table: "Wypozyczenie");

            migrationBuilder.DropIndex(
                name: "IX_Rezerwacja_UzytkownikId",
                table: "Rezerwacja");

            migrationBuilder.AlterColumn<int>(
                name: "UzytkownikId",
                table: "Wypozyczenie",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UzytkownikId1",
                table: "Wypozyczenie",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "UzytkownikId",
                table: "Rezerwacja",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UzytkownikId1",
                table: "Rezerwacja",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wypozyczenie_UzytkownikId1",
                table: "Wypozyczenie",
                column: "UzytkownikId1");

            migrationBuilder.CreateIndex(
                name: "IX_Rezerwacja_UzytkownikId1",
                table: "Rezerwacja",
                column: "UzytkownikId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Rezerwacja_AspNetUsers_UzytkownikId1",
                table: "Rezerwacja",
                column: "UzytkownikId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wypozyczenie_AspNetUsers_UzytkownikId1",
                table: "Wypozyczenie",
                column: "UzytkownikId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
