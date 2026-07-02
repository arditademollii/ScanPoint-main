using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class ProvimiFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nxenesit");

            migrationBuilder.DropTable(
                name: "Shkollat");

            migrationBuilder.CreateTable(
                name: "Postimet",
                columns: table => new
                {
                    ID_Postimi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmriAutorit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postimet", x => x.ID_Postimi);
                });

            migrationBuilder.CreateTable(
                name: "Komentet",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_Postimi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Komentet", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Komentet_Postimet_ID_Postimi",
                        column: x => x.ID_Postimi,
                        principalTable: "Postimet",
                        principalColumn: "ID_Postimi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Komentet_ID_Postimi",
                table: "Komentet",
                column: "ID_Postimi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Komentet");

            migrationBuilder.DropTable(
                name: "Postimet");

            migrationBuilder.CreateTable(
                name: "Shkollat",
                columns: table => new
                {
                    ID_Shkolla = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriShkolles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qyteti = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shkollat", x => x.ID_Shkolla);
                });

            migrationBuilder.CreateTable(
                name: "Nxenesit",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Shkolla = table.Column<int>(type: "int", nullable: false),
                    EmriNxenesit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Klasa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nxenesit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Nxenesit_Shkollat_ID_Shkolla",
                        column: x => x.ID_Shkolla,
                        principalTable: "Shkollat",
                        principalColumn: "ID_Shkolla",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Nxenesit_ID_Shkolla",
                table: "Nxenesit",
                column: "ID_Shkolla");
        }
    }
}
