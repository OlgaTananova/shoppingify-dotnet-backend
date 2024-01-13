using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models.ValidationModels
{
    public class ChangeItemPricePerUnitModel
    {
        [Required]
        public required string ShoppingListId { get; set; }
        [Required]
        public required string ShoppingListItemId { get; set; }
        [Required]
        public required decimal PricePerUnit { get; set; }
    }
}
