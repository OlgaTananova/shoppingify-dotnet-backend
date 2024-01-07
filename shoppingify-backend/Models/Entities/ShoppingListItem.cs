using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace shoppingify_backend.Models.Entities
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
        public required Guid ShoppingListId { get; set; }

        [ForeignKey("ShoppingListId")]
        public required ShoppingList ShoppingList { get; set; }

        public required Guid ItemId { get; set; }

        [ForeignKey("ItemId")]
        public required Item Item { get; set; }

        public required Guid CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public required Category Category { get; set; }

        public required Guid OwnerId { get; set; }
        public decimal Quantity { get; set; } = 1.00M;
        public string Units { get; set; } = "pcs";
        public decimal PricePerUnit { get; set; } = 0.00M;
        public decimal Price { get; set; } = 0.00M;
        public ItemStatus Status { get; set; } = ItemStatus.Pending;
        public bool IsDeleted { get; set; } = false;

    }
}
