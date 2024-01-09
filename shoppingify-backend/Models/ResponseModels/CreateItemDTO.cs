namespace shoppingify_backend.Models.ResponseModels
{
    public class CreateItemDTO
    {
        public required CategoryDTO Category { get; set; }
        public required ItemDTO Item { get; set; }
    }
}
