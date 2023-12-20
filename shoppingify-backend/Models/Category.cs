using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace shoppingify_backend.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Item> Items { get; set; }
        public string OwnerId { get; set; }

        public Category()
        {
            Items = new HashSet<Item>();
        }

    }
}
