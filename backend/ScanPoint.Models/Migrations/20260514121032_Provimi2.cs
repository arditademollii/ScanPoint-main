using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class Provimi2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fabrikat",
                columns: table => new
                {
                    ID_Fabrika = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriFabrikes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lokacioni = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    MbriemriPunetorit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pozita = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
        }
    }
}
