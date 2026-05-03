using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class Provimi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    EmriNxenesit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Klasa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_Shkolla = table.Column<int>(type: "int", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nxenesit");

            migrationBuilder.DropTable(
                name: "Shkollat");
        }
    }
}
