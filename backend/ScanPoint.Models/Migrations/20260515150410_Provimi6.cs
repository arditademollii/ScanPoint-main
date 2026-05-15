using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class Provimi6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Teams232470351",
                columns: table => new
                {
                    ID_Team232470351 = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriTeam232470351 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams232470351", x => x.ID_Team232470351);
                });

            migrationBuilder.CreateTable(
                name: "Players232470351",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriPlayer232470351 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_Team232470351 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players232470351", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Players232470351_Teams232470351_ID_Team232470351",
                        column: x => x.ID_Team232470351,
                        principalTable: "Teams232470351",
                        principalColumn: "ID_Team232470351",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players232470351_ID_Team232470351",
                table: "Players232470351",
                column: "ID_Team232470351");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players232470351");

            migrationBuilder.DropTable(
                name: "Teams232470351");
        }
    }
}
