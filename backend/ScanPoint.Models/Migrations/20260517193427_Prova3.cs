using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class Prova3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Punetoret");

            migrationBuilder.DropTable(
                name: "Fabrikat");

            migrationBuilder.CreateTable(
                name: "Ligjeruesit",
                columns: table => new
                {
                    ID_Ligjeruesi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriLigjeruesit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Departamenti = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ligjeruesit", x => x.ID_Ligjeruesi);
                });

            migrationBuilder.CreateTable(
                name: "Ligjeratat",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriLigjerates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_Ligjeruesi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ligjeratat", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Ligjeratat_Ligjeruesit_ID_Ligjeruesi",
                        column: x => x.ID_Ligjeruesi,
                        principalTable: "Ligjeruesit",
                        principalColumn: "ID_Ligjeruesi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ligjeratat_ID_Ligjeruesi",
                table: "Ligjeratat",
                column: "ID_Ligjeruesi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ligjeratat");

            migrationBuilder.DropTable(
                name: "Ligjeruesit");

            migrationBuilder.CreateTable(
                name: "Fabrikat",
                columns: table => new
                {
                    ID_Fabrika = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriFabrikes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MbiemriFabrikes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pozita = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fabrikat", x => x.ID_Fabrika);
                });

            migrationBuilder.CreateTable(
                name: "Punetoret",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Fabrika = table.Column<int>(type: "int", nullable: false),
                    EmriPunetorit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lokacioni = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Punetoret", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Punetoret_Fabrikat_ID_Fabrika",
                        column: x => x.ID_Fabrika,
                        principalTable: "Fabrikat",
                        principalColumn: "ID_Fabrika",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Punetoret_ID_Fabrika",
                table: "Punetoret",
                column: "ID_Fabrika");
        }
    }
}
