using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class FixProvimi3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ligjerata_Ligjeruesi_ID_Ligjeruesi",
                table: "Ligjerata");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ligjeruesi",
                table: "Ligjeruesi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ligjerata",
                table: "Ligjerata");

            migrationBuilder.RenameTable(
                name: "Ligjeruesi",
                newName: "Ligjeruesit");

            migrationBuilder.RenameTable(
                name: "Ligjerata",
                newName: "Ligjeratat");

            migrationBuilder.RenameIndex(
                name: "IX_Ligjerata_ID_Ligjeruesi",
                table: "Ligjeratat",
                newName: "IX_Ligjeratat_ID_Ligjeruesi");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ligjeruesit",
                table: "Ligjeruesit",
                column: "ID_Lecturer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ligjeratat",
                table: "Ligjeratat",
                column: "ID_Ligjerata");

            migrationBuilder.AddForeignKey(
                name: "FK_Ligjeratat_Ligjeruesit_ID_Ligjeruesi",
                table: "Ligjeratat",
                column: "ID_Ligjeruesi",
                principalTable: "Ligjeruesit",
                principalColumn: "ID_Lecturer",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ligjeratat_Ligjeruesit_ID_Ligjeruesi",
                table: "Ligjeratat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ligjeruesit",
                table: "Ligjeruesit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ligjeratat",
                table: "Ligjeratat");

            migrationBuilder.RenameTable(
                name: "Ligjeruesit",
                newName: "Ligjeruesi");

            migrationBuilder.RenameTable(
                name: "Ligjeratat",
                newName: "Ligjerata");

            migrationBuilder.RenameIndex(
                name: "IX_Ligjeratat_ID_Ligjeruesi",
                table: "Ligjerata",
                newName: "IX_Ligjerata_ID_Ligjeruesi");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ligjeruesi",
                table: "Ligjeruesi",
                column: "ID_Lecturer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ligjerata",
                table: "Ligjerata",
                column: "ID_Ligjerata");

            migrationBuilder.AddForeignKey(
                name: "FK_Ligjerata_Ligjeruesi_ID_Ligjeruesi",
                table: "Ligjerata",
                column: "ID_Ligjeruesi",
                principalTable: "Ligjeruesi",
                principalColumn: "ID_Lecturer",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
