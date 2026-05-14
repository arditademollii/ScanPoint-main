using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class FixSyntaxErrorr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MbriemriPunetorit",
                table: "Punetoret",
                newName: "MbiemriPunetorit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MbiemriPunetorit",
                table: "Punetoret",
                newName: "MbriemriPunetorit");
        }
    }
}
