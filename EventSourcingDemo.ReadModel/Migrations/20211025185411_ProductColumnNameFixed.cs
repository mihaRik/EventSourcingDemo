using Microsoft.EntityFrameworkCore.Migrations;

namespace EventSourcingDemo.ReadModel.Migrations
{
    public partial class ProductColumnNameFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descpription",
                table: "BaseItems",
                newName: "Description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "BaseItems",
                newName: "Descpription");
        }
    }
}
