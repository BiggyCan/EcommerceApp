using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EcommerceApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackedCarts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackedCarts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConvertedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConvertedOrderId = table.Column<int>(type: "integer", nullable: true),
                    TotalItems = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackedCarts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackedCarts_Orders_ConvertedOrderId",
                        column: x => x.ConvertedOrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TrackedCartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrackedCartId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedCartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackedCartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackedCartItems_TrackedCarts_TrackedCartId",
                        column: x => x.TrackedCartId,
                        principalTable: "TrackedCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackedCartItems_ProductId",
                table: "TrackedCartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedCartItems_TrackedCartId",
                table: "TrackedCartItems",
                column: "TrackedCartId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedCarts_ConvertedOrderId",
                table: "TrackedCarts",
                column: "ConvertedOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedCarts_LastActivityAt",
                table: "TrackedCarts",
                column: "LastActivityAt");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedCarts_Status",
                table: "TrackedCarts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedCarts_UserId",
                table: "TrackedCarts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedCarts_UserId_Status",
                table: "TrackedCarts",
                columns: new[] { "UserId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackedCartItems");

            migrationBuilder.DropTable(
                name: "TrackedCarts");
        }
    }
}
