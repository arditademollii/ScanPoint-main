using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class ProvimiCH1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Universitetet",
                columns: table => new
                {
                    ID_Universiteti = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriUniversitetit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Shteti = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universitetet", x => x.ID_Universiteti);
                });

            migrationBuilder.CreateTable(
                name: "Profesoret",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriProfesorit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lenda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_Universiteti = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profesoret", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Profesoret_Universitetet_ID_Universiteti",
                        column: x => x.ID_Universiteti,
                        principalTable: "Universitetet",
                        principalColumn: "ID_Universiteti",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profesoret_ID_Universiteti",
                table: "Profesoret",
                column: "ID_Universiteti");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profesoret");

            migrationBuilder.DropTable(
                name: "Universitetet");
        }
    }
}
