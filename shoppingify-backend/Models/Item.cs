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
        public string ItemName { get; set; }
        public Guid CategoryId { get; set; }
        public string OwnerId { get; set; }
        public string Image { get; set; } = "";
        public string Note { get; set; } = "";
        public bool IsDeleted { get; set; } = false;

        // Navigation property
        [ForeignKey("CategoryId")]
        [JsonIgnore]
        public Category Category { get; set; }

    }
}
