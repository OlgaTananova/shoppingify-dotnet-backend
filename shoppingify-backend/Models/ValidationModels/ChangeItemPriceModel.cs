using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models.ValidationModels
{
    public class ChangeItemPriceModel
    {
        [Required]
        public required string ShoppingListId { get; set; }
        [Required]
        public required string ShoppingListItemId { get; set; }
        [Required]
        public required decimal Price { get; set; }
    }
}
