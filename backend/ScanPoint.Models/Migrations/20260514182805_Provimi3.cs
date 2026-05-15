using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class Provimi3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ligjeruesi",
                columns: table => new
                {
                    ID_Lecturer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriLigjeruesit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Departamenti = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ligjeruesi", x => x.ID_Lecturer);
                });

            migrationBuilder.CreateTable(
                name: "Ligjerata",
                columns: table => new
                {
                    ID_Ligjerata = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriLigjerates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_Ligjeruesi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ligjerata", x => x.ID_Ligjerata);
                    table.ForeignKey(
                        name: "FK_Ligjerata_Ligjeruesi_ID_Ligjeruesi",
                        column: x => x.ID_Ligjeruesi,
                        principalTable: "Ligjeruesi",
                        principalColumn: "ID_Lecturer",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ligjerata_ID_Ligjeruesi",
                table: "Ligjerata",
                column: "ID_Ligjeruesi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ligjerata");

            migrationBuilder.DropTable(
                name: "Ligjeruesi");
        }
    }
}
