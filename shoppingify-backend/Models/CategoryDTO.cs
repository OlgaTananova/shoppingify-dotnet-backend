namespace shoppingify_backend.Models
{
    public class CategoryDTO
    {
        public string _id { get; set; }
        public string Category { get; set; }
        public string Owner { get; set; }
        public List<string> Items { get; set; }
    }
}
