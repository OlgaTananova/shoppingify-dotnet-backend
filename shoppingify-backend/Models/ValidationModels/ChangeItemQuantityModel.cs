using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models.ValidationModels
{
    public class ChangeItemQuantityModel
    {
        [Required]
        public required string ShoppingListId { get; set; }
        [Required]
        public required string ShoppingListItemId { get; set; }
        [Required]
        public required decimal Quantity { get; set; }
    }
}
