using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text.Json.Serialization;

namespace shoppingify_backend.Models.Entities
{
    public enum ShoppingListStatus
    {
        Active,
        Completed,
        Cancelled,
        Idle,
        Deleted
    }
    public class ShoppingList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Heading { get; set; } = "Shopping List";
        public DateTime Date { get; set; } = DateTime.Now;
        public required Guid OwnerId { get; set; }
        public ShoppingListStatus Status { get; set; } = ShoppingListStatus.Active;

        //Navigation Property
        public List<ShoppingListItem> ShoppingListItems { get; set; } = new List<ShoppingListItem>();
        public decimal SalesTax { get; set; } = 0.00M;
        public bool IsDeleted { get; set; } = false;

    }
}
