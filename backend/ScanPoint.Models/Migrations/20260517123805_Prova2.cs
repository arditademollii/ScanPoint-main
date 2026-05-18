using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class Prova2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Studentet");

            migrationBuilder.DropTable(
                name: "Kurset");

            migrationBuilder.CreateTable(
                name: "Bibliotekat",
                columns: table => new
                {
                    ID_Biblioteka = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriBibliotekes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    TitulliLibrit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Autori = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumriFaqeve = table.Column<int>(type: "int", nullable: false),
                    ID_Biblioteka = table.Column<int>(type: "int", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Librat");

            migrationBuilder.DropTable(
                name: "Bibliotekat");

            migrationBuilder.CreateTable(
                name: "Kurset",
                columns: table => new
                {
                    ID_Kursi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriKursit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kohezgjatja = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pershkrimi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kurset", x => x.ID_Kursi);
                });

            migrationBuilder.CreateTable(
                name: "Studentet",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Kursi = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmriStudentit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MbiemriStudentit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studentet", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Studentet_Kurset_ID_Kursi",
                        column: x => x.ID_Kursi,
                        principalTable: "Kurset",
                        principalColumn: "ID_Kursi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Studentet_ID_Kursi",
                table: "Studentet",
                column: "ID_Kursi");
        }
    }
}
