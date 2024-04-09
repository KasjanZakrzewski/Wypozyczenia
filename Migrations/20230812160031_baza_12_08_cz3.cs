using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WypozyczeniaAPI.Migrations
{
    public partial class baza_12_08_cz3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pojazd",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rejestracja = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Marka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NS = table.Column<float>(type: "real", nullable: false),
                    WE = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pojazd", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rezerwacja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataRozpoczęcia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataZakończenia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PojazdId = table.Column<int>(type: "int", nullable: false),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false),
                    UzytkownikId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezerwacja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rezerwacja_AspNetUsers_UzytkownikId1",
                        column: x => x.UzytkownikId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rezerwacja_Pojazd_PojazdId",
                        column: x => x.PojazdId,
                        principalTable: "Pojazd",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wypozyczenie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataRozpoczęcia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataZakończenia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PojazdId = table.Column<int>(type: "int", nullable: false),
                    UzytkownikId = table.Column<int>(type: "int", nullable: false),
                    UzytkownikId1 = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wypozyczenie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wypozyczenie_AspNetUsers_UzytkownikId1",
                        column: x => x.UzytkownikId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wypozyczenie_Pojazd_PojazdId",
                        column: x => x.PojazdId,
                        principalTable: "Pojazd",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pozycja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NS = table.Column<float>(type: "real", nullable: false),
                    WE = table.Column<float>(type: "real", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WypozyczenieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pozycja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pozycja_Wypozyczenie_WypozyczenieId",
                        column: x => x.WypozyczenieId,
                        principalTable: "Wypozyczenie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pozycja_WypozyczenieId",
                table: "Pozycja",
                column: "WypozyczenieId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezerwacja_PojazdId",
                table: "Rezerwacja",
                column: "PojazdId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezerwacja_UzytkownikId1",
                table: "Rezerwacja",
                column: "UzytkownikId1");

            migrationBuilder.CreateIndex(
                name: "IX_Wypozyczenie_PojazdId",
                table: "Wypozyczenie",
                column: "PojazdId");

            migrationBuilder.CreateIndex(
                name: "IX_Wypozyczenie_UzytkownikId1",
                table: "Wypozyczenie",
                column: "UzytkownikId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pozycja");

            migrationBuilder.DropTable(
                name: "Rezerwacja");

            migrationBuilder.DropTable(
                name: "Wypozyczenie");

            migrationBuilder.DropTable(
                name: "Pojazd");
        }
    }
}
