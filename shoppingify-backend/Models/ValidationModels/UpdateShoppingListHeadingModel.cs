using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models.ValidationModels
{
    public class UpdateShoppingListHeadingModel
    {

        [Required]
        public required string ShoppingListId { get; set; }

        [Required]
        public required string Heading { get; set; }

    }
}
