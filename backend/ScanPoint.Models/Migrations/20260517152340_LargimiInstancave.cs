using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class LargimiInstancave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Ligjeratat");

            migrationBuilder.DropTable(
                name: "Players232470351");

            migrationBuilder.DropTable(
                name: "Profesoret");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Ligjeruesit");

            migrationBuilder.DropTable(
                name: "Teams232470351");

            migrationBuilder.DropTable(
                name: "Universitetet");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    ID_Employee = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriEmployee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MbiemriEmployee = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.ID_Employee);
                });

            migrationBuilder.CreateTable(
                name: "Ligjeruesit",
                columns: table => new
                {
                    ID_Ligjeruesi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Departamenti = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmriLigjeruesit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ligjeruesit", x => x.ID_Ligjeruesi);
                });

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
                name: "Contracts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Employee = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Contracts_Employees_ID_Employee",
                        column: x => x.ID_Employee,
                        principalTable: "Employees",
                        principalColumn: "ID_Employee",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ligjeratat",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Ligjeruesi = table.Column<int>(type: "int", nullable: false),
                    EmriLigjerates = table.Column<string>(type: "nvarchar(max)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Players232470351",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Team232470351 = table.Column<int>(type: "int", nullable: false),
                    EmriPlayer232470351 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Profesoret",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Universiteti = table.Column<int>(type: "int", nullable: false),
                    EmriProfesorit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lenda = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                name: "IX_Contracts_ID_Employee",
                table: "Contracts",
                column: "ID_Employee");

            migrationBuilder.CreateIndex(
                name: "IX_Ligjeratat_ID_Ligjeruesi",
                table: "Ligjeratat",
                column: "ID_Ligjeruesi");

            migrationBuilder.CreateIndex(
                name: "IX_Players232470351_ID_Team232470351",
                table: "Players232470351",
                column: "ID_Team232470351");

            migrationBuilder.CreateIndex(
                name: "IX_Profesoret_ID_Universiteti",
                table: "Profesoret",
                column: "ID_Universiteti");
        }
    }
}
