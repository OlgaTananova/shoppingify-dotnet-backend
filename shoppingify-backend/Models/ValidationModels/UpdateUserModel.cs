using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models.ValidationModels
{
    public class UpdateUserModel
    {
        [MinLength(2)]
        [MaxLength(30)]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

    }
}
