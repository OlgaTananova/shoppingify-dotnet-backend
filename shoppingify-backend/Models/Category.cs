using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace shoppingify_backend.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public required string CategoryName { get; set; }
        public Guid OwnerId { get; set; }

        //Navigation properties
        public List<Item> Items { get; set; } = new List<Item>();
        public List<ShoppingListItem> ShoppingListItems { get; set; } = new List<ShoppingListItem>();


    }
}
