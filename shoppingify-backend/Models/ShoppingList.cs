using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace shoppingify_backend.Models
{
    public enum ShoppingListStatus
    {
        Active,
        Completed,
        Cancelled, 
        Idle
    }
    public class ShoppingList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Heading { get; set; } = "Shopping List";
        public DateTime Date { get; set; }
        public Guid OwnerId { get; set; }
        public ShoppingListStatus Status { get; set; } = ShoppingListStatus.Active;

        public decimal SalesTax { get; set; } = 0.00M;


    }
}
