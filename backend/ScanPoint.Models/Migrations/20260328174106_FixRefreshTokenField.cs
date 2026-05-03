using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanPoint.Models.Migrations
{
    /// <inheritdoc />
    public partial class FixRefreshTokenField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuyXGetYRules");

            migrationBuilder.DropTable(
                name: "CartTotalDiscountRules");

            migrationBuilder.DropTable(
                name: "ExpirationDiscountRules");

            migrationBuilder.DropTable(
                name: "MaxDiscountLimitRules");

            migrationBuilder.DropTable(
                name: "ProductDiscountRules");

            migrationBuilder.DropTable(
                name: "ProductTimeBasedDiscountRules");

            migrationBuilder.DropTable(
                name: "QuantityDiscountRules");

            migrationBuilder.DropTable(
                name: "DiscountRules");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Shops",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Products",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Shops_Name_AdminId",
                table: "Shops",
                columns: new[] { "Name", "AdminId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode_ShopId",
                table: "Products",
                columns: new[] { "Barcode", "ShopId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shops_Name_AdminId",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Products_Barcode_ShopId",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Shops",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "DiscountRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BuyXGetYRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductXId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductYId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    XQuantity = table.Column<int>(type: "int", nullable: false),
                    YQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyXGetYRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuyXGetYRules_DiscountRules_Id",
                        column: x => x.Id,
                        principalTable: "DiscountRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartTotalDiscountRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinCartTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartTotalDiscountRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartTotalDiscountRules_DiscountRules_Id",
                        column: x => x.Id,
                        principalTable: "DiscountRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpirationDiscountRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DaysBeforeExpiry = table.Column<int>(type: "int", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpirationDiscountRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpirationDiscountRules_DiscountRules_Id",
                        column: x => x.Id,
                        principalTable: "DiscountRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaxDiscountLimitRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaxDiscountLimitRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaxDiscountLimitRules_DiscountRules_Id",
                        column: x => x.Id,
                        principalTable: "DiscountRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductDiscountRules",
                columns: table => new
                {
                    DiscountRulesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDiscountRules", x => new { x.DiscountRulesId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_ProductDiscountRules_DiscountRules_DiscountRulesId",
                        column: x => x.DiscountRulesId,
                        principalTable: "DiscountRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductDiscountRules_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductTimeBasedDiscountRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTimeBasedDiscountRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductTimeBasedDiscountRules_DiscountRules_Id",
                        column: x => x.Id,
                        principalTable: "DiscountRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuantityDiscountRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinQuantity = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuantityDiscountRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuantityDiscountRules_DiscountRules_Id",
                        column: x => x.Id,
                        principalTable: "DiscountRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDiscountRules_ProductsId",
                table: "ProductDiscountRules",
                column: "ProductsId");
        }
    }
}
