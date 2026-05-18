using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class Prova11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Librat");

            migrationBuilder.DropTable(
                name: "Bibliotekat");

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
                    EmriPunetorit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lokacioni = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_Fabrika = table.Column<int>(type: "int", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Punetoret");

            migrationBuilder.DropTable(
                name: "Fabrikat");

            migrationBuilder.CreateTable(
                name: "Bibliotekat",
                columns: table => new
                {
                    ID_Biblioteka = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmriBibliotekes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bibliotekat", x => x.ID_Biblioteka);
                });

            migrationBuilder.CreateTable(
                name: "Librat",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Biblioteka = table.Column<int>(type: "int", nullable: false),
                    Autori = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumriFaqeve = table.Column<int>(type: "int", nullable: false),
                    TitulliLibrit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Librat", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Librat_Bibliotekat_ID_Biblioteka",
                        column: x => x.ID_Biblioteka,
                        principalTable: "Bibliotekat",
                        principalColumn: "ID_Biblioteka",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Librat_ID_Biblioteka",
                table: "Librat",
                column: "ID_Biblioteka");
        }
    }
}
