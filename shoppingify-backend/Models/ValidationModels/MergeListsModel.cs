using shoppingify_backend.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models.ValidationModels
{
    public class MergeListsModel
    {
        [Required]
        public required string _id { get; set; }

        [Required]
        public required decimal SalesTax  { get; set; }

        [Required]
        public required string Date { get; set; }

        [Required]
        public required List<MergeListItemModel> Items { get; set; }

    }

    public class MergeListItemModel
    {
        [Required]
        public required string _id { get; set; }

        [Required]
        public required string ItemId { get; set; }

        [Required]
        public required string CategoryId { get; set; }

        [Required]
        public required decimal Quantity { get; set; }

        [Required]
        public required string Units { get; set; }

        [Required]
        public required string Status { get; set; }

        [Required]
        public required decimal PricePerUnit { get; set; }

        [Required]
        public required decimal Price { get; set; }
    }
}
