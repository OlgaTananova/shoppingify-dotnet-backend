using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models
{
    public class ShoppingListModel
    {
        [Required]
        public string CategoryId { get; set; }

        [Required]
        public string ItemId { get; set; }

    }
}
