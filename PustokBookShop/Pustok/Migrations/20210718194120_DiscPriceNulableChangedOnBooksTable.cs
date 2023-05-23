using Microsoft.EntityFrameworkCore.Migrations;

namespace Pustok.Migrations
{
    public partial class DiscPriceNulableChangedOnBooksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountedPrice",
                table: "Books",
                type: "Money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "Money");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountedPrice",
                table: "Books",
                type: "Money",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "Money",
                oldNullable: true);
        }
    }
}
