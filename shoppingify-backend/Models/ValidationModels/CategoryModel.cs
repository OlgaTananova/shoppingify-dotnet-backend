using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models.ValidationModels
{
    public class CategoryModel
    {

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string Category { get; set; }

    }
}
