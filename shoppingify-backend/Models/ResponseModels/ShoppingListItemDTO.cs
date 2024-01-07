using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Eventing.Reader;
using System.Text.Json.Serialization;

namespace shoppingify_backend.Models.ResponseModels
{
    public class ShoppingListItemDTO
    {
        public required string _id { get; set; }
        public required string ItemId { get; set; }

        public required string CategoryId { get; set; }

        public required decimal Quantity { get; set; }
        public required string Status { get; set; }
        public required string Units { get; set; }
        public required decimal PricePerUnit { get; set; }
        public required decimal Price { get; set; }
        public required bool IsDeleted { get; set; }

    }
}
