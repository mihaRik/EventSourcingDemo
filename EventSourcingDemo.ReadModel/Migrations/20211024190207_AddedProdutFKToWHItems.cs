using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventSourcingDemo.ReadModel.Migrations
{
    public partial class AddedProdutFKToWHItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "WarehouseItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "WarehouseItems");
        }
    }
}
