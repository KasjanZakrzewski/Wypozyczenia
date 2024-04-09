using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WypozyczeniaAPI.Migrations
{
    public partial class Serwis_Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Serwis",
                newName: "DataRozpoczecia");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataZakonczenia",
                table: "Serwis",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataZakonczenia",
                table: "Serwis");

            migrationBuilder.RenameColumn(
                name: "DataRozpoczecia",
                table: "Serwis",
                newName: "Data");
        }
    }
}
