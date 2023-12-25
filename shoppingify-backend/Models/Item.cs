using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace shoppingify_backend.Models
{
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public required string ItemName { get; set; }
        public Guid CategoryId { get; set; }
        public Guid OwnerId { get; set; }
        public string Image { get; set; } = "";
        public string Note { get; set; } = "";
        public bool IsDeleted { get; set; } = false;

        // Navigation property
        [ForeignKey("CategoryId")]
        public required Category Category { get; set; }

        // Navigation property
        public List<ShoppingListItem> ShoppingListItems { get; set; } = new List<ShoppingListItem>();

    }
}
