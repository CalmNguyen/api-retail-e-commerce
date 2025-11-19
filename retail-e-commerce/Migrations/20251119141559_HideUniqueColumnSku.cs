using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace retail_e_commerce.Migrations
{
    /// <inheritdoc />
    public partial class HideUniqueColumnSku : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_Sku",
                table: "ProductVariants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_Sku",
                table: "ProductVariants",
                column: "Sku",
                unique: true);
        }
    }
}
