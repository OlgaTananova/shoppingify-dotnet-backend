namespace shoppingify_backend.Models.ResponseModels
{
    public class CategoryDTO
    {
        public required string _id { get; set; }
        public required string Category { get; set; }
        public required string Owner { get; set; }
        public required List<string> Items { get; set; } = new List<string>();
    }
}
