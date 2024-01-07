using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models.ValidationModels
{
    public class DeleteItemFromShoppingListModel
    {
        [Required]
        public required string ShoppingListId { get; set; }

        [Required]
        public required string ShoppingListItemId { get; set; }

    }
}
