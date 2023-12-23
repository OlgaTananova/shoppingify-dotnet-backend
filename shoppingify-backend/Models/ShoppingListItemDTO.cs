using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace shoppingify_backend.Models
{
    public class ShoppingListItemDTO
    {
        public string ItemId { get; set; }

        public string CategoryId { get; set; }

        public decimal Quantity { get; set; }
        public string Status { get; set; }
        public string Units { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal Price { get; set; }

    }
}
