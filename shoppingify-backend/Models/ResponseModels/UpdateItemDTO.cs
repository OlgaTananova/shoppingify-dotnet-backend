namespace shoppingify_backend.Models.ResponseModels
{
    public class UpdateItemDTO
    {
        public required ItemDTO UpdatedItem { get; set; }
        public required List<CategoryDTO> UpdatedCategories { get; set; }
        public required List<ShoppingListDTO> UpdatedShoppingLists { get; set; }
    }
}
