using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace shoppingify_backend.Models
{
    public enum ItemStatus
    {
        Pending,
        Completed,
    };
    public class ShoppingListItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid ShoppingListId { get; set; }
        
        [ForeignKey("ShoppingListId")]
        [JsonIgnore]
        public ShoppingList ShoppingList { get; set; }

        public Guid ItemId { get; set; }

        [ForeignKey("ItemId")]
        [JsonIgnore]
        public Item Item { get; set; }

        public Guid CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [JsonIgnore]
        public Category Category { get; set; }

        public Guid OwnerId { get; set; }
        public decimal Quantity { get; set; } = 0.00M;
        public string Units { get; set; } = "pcs";
        public decimal PricePerUnit { get; set; } = 0.00M;
        public decimal Price { get; set; } = 0.00M;
        public ItemStatus Status { get; set; } = ItemStatus.Pending;

    }
}
