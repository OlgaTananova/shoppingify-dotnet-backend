using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models.ValidationModels
{
    public class UpdateShoppingListStatusModel
    {
        [Required]
        public required string ShoppingListId { get; set; }

        [Required]
        public required string Status { get; set; }
    }
}
