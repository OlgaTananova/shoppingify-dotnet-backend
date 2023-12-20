using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models
{
    public class CategoryModel
    {

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string Category { get; set; }

    }
}
