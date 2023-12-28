using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models.Entities
{
    public class ShoppingListModel
    {
        [Required]
        public string CategoryId { get; set; }

        [Required]
        public string ItemId { get; set; }

    }
}
