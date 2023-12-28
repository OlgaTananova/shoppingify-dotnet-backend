using System.ComponentModel.DataAnnotations;

namespace shoppingify_backend.Models.ResponseModels
{
    public class ItemDTO
    {

        public required string _id { get; set; }
        public required string Name { get; set; }
        public required string CategoryId { get; set; }
        public required string Owner { get; set; }
        public required string Note { get; set; }
        public required string Image { get; set; }
        public required bool IsDeleted { get; set; }

    }
}
