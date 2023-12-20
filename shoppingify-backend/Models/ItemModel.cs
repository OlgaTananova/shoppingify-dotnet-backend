using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models
{
    public class ItemModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public string CategoryId { get; set; }
        public string Image { get; set; } = "";

        public string Note { get; set; } = "";

    }
}
