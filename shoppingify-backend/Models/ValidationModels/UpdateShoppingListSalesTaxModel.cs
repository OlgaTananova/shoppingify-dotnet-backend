using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models.ValidationModels
{
    public class UpdateShoppingListSalesTaxModel
    {

        [Required]
        public required string ShoppingListId { get; set; }

        [Required]
        public required decimal SalesTax { get; set; }
    }
}
