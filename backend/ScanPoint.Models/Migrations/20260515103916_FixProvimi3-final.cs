using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class FixProvimi3final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID_Lecturer",
                table: "Ligjeruesit",
                newName: "ID_Ligjeruesi");

            migrationBuilder.RenameColumn(
                name: "ID_Ligjerata",
                table: "Ligjeratat",
                newName: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID_Ligjeruesi",
                table: "Ligjeruesit",
                newName: "ID_Lecturer");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Ligjeratat",
                newName: "ID_Ligjerata");
        }
    }
}
