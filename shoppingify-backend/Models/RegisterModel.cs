using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models
{
    public class RegisterModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
