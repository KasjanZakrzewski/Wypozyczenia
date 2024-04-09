using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WypozyczeniaAPI.Migrations
{
    public partial class SerwisUpDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Serwis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PojazdId = table.Column<int>(type: "int", nullable: false),
                    AdminId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PracownikId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Serwis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Serwis_AspNetUsers_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Serwis_AspNetUsers_PracownikId",
                        column: x => x.PracownikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Serwis_Pojazd_PojazdId",
                        column: x => x.PojazdId,
                        principalTable: "Pojazd",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Opis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tytul = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Treść = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerwisId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Opis_Serwis_SerwisId",
                        column: x => x.SerwisId,
                        principalTable: "Serwis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Opis_SerwisId",
                table: "Opis",
                column: "SerwisId");

            migrationBuilder.CreateIndex(
                name: "IX_Serwis_AdminId",
                table: "Serwis",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Serwis_PojazdId",
                table: "Serwis",
                column: "PojazdId");

            migrationBuilder.CreateIndex(
                name: "IX_Serwis_PracownikId",
                table: "Serwis",
                column: "PracownikId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Opis");

            migrationBuilder.DropTable(
                name: "Serwis");
        }
    }
}
