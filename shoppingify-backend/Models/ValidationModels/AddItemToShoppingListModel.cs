using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models.ValidationModels
{
    public class AddItemToShoppingListModel
    {
        [Required]
        public required string ShoppingListId { get; set; }

        [Required]
        public required string ItemId { get; set; }

        [Required]
        public required string CategoryId { get; set; }

        public string Status { get; set; } = "Pending";

        public decimal Quantity { get; set; } = 1.00M;
    }
}
