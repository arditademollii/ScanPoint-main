using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class AtributeNdryshe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Spitalet",
                columns: table => new
                {
                    ID_Spitali = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriSpitalit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumriKateve = table.Column<int>(type: "int", nullable: false),
                    KaUrgjence = table.Column<bool>(type: "bit", nullable: false),
                    DataHapjes = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spitalet", x => x.ID_Spitali);
                });

            migrationBuilder.CreateTable(
                name: "Mjeket",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriMjekut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Paga = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataPunesimit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EshteSpecialist = table.Column<bool>(type: "bit", nullable: false),
                    ID_Spitali = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mjeket", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Mjeket_Spitalet_ID_Spitali",
                        column: x => x.ID_Spitali,
                        principalTable: "Spitalet",
                        principalColumn: "ID_Spitali",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mjeket_ID_Spitali",
                table: "Mjeket",
                column: "ID_Spitali");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mjeket");

            migrationBuilder.DropTable(
                name: "Spitalet");
        }
    }
}
