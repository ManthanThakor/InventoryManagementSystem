using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerItem_CustomerID",
                table: "CustomerItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerItem_ItemID",
                table: "CustomerItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerItem_SalesOrderID",
                table: "CustomerItems");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerItem_CustomerID",
                table: "CustomerItems",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerItem_ItemID",
                table: "CustomerItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerItem_SalesOrderID",
                table: "CustomerItems",
                column: "SalesOrderId",
                principalTable: "SalesOrders",
                principalColumn: "SalesOrderId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerItem_CustomerID",
                table: "CustomerItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerItem_ItemID",
                table: "CustomerItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerItem_SalesOrderID",
                table: "CustomerItems");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerItem_CustomerID",
                table: "CustomerItems",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerItem_ItemID",
                table: "CustomerItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerItem_SalesOrderID",
                table: "CustomerItems",
                column: "SalesOrderId",
                principalTable: "SalesOrders",
                principalColumn: "SalesOrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
