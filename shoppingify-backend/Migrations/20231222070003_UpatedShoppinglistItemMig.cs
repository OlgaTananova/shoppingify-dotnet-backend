using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shoppingifybackend.Migrations.Application
{
    /// <inheritdoc />
    public partial class UpatedShoppinglistItemMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShoppingListItems",
                table: "ShoppingListItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShoppingListItems",
                table: "ShoppingListItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingListItems_ShoppingListId",
                table: "ShoppingListItems",
                column: "ShoppingListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShoppingListItems",
                table: "ShoppingListItems");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingListItems_ShoppingListId",
                table: "ShoppingListItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShoppingListItems",
                table: "ShoppingListItems",
                columns: new[] { "ShoppingListId", "ItemId", "CategoryId" });
        }
    }
}
