using shoppingify_backend.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models.ValidationModels
{
    public class UploadShoppingListModel
    {
        public required decimal SalesTax { get; set; }
        public required string Date { get; set; }

        public required List<UploadShoppingListItemModel> Items { get; set; }
    }

    public class UploadShoppingListItemModel
    {
        [Required]
        public required string CategoryId { get; set; }
        [Required]
        public required string ItemId { get; set; }
        [Required]
        public decimal Quantity { get; set; } = 1.0M;
        [Required]
        public string Status { get; set; } = "completed";
        [Required]
        public required string Units { get; set; }
        public decimal PricePerUnit { get; set; } = 0.0M;
        public decimal Price { get; set; } = 0.0M;

    }
}
