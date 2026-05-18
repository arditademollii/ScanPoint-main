using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class ProvaFinale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ligjeratat");

            migrationBuilder.DropTable(
                name: "Ligjeruesit");

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    ID_Employee = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmriEmployee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MbiemriEmployee = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.ID_Employee);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pershkrimi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titulli = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_Employee = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Contracts_Employee_ID_Employee",
                        column: x => x.ID_Employee,
                        principalTable: "Employee",
                        principalColumn: "ID_Employee",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ID_Employee",
                table: "Contracts",
                column: "ID_Employee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Employee");

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

            migrationBuilder.CreateIndex(
                name: "IX_Ligjeratat_ID_Ligjeruesi",
                table: "Ligjeratat",
                column: "ID_Ligjeruesi");
        }
    }
}
