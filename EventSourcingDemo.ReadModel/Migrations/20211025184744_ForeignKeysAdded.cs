using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventSourcingDemo.ReadModel.Migrations
{
    public partial class ForeignKeysAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseItems_Warehouses_WarehouseReadModelId",
                table: "WarehouseItems");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseItems_WarehouseReadModelId",
                table: "WarehouseItems");

            migrationBuilder.DropColumn(
                name: "WarehouseReadModelId",
                table: "WarehouseItems");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseItems_ProductId",
                table: "WarehouseItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseItems_WarehouseId",
                table: "WarehouseItems",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseItems_BaseItems_ProductId",
                table: "WarehouseItems",
                column: "ProductId",
                principalTable: "BaseItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseItems_Warehouses_WarehouseId",
                table: "WarehouseItems",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseItems_BaseItems_ProductId",
                table: "WarehouseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseItems_Warehouses_WarehouseId",
                table: "WarehouseItems");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseItems_ProductId",
                table: "WarehouseItems");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseItems_WarehouseId",
                table: "WarehouseItems");

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseReadModelId",
                table: "WarehouseItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseItems_WarehouseReadModelId",
                table: "WarehouseItems",
                column: "WarehouseReadModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseItems_Warehouses_WarehouseReadModelId",
                table: "WarehouseItems",
                column: "WarehouseReadModelId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
