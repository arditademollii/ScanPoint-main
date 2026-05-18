using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class SetupFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Employee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                    ID_Employee = table.Column<int>(type: "int", nullable: false),
                    Pershkrimi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titulli = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
    }
}
