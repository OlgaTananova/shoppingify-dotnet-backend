using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models
{
    public class ItemDTO
    {

        public string _id { get; set; }
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public string Owner { get; set; }
        public string Note { get; set; }
        public string Image { get; set; }

    }
}
