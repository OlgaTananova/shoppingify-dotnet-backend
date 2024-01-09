namespace shoppingify_backend.Models.ResponseModels
{
    public class DeleteItemDTO
    {
        public required string Message { get; set; }
        public required ItemDTO DeletedItem { get; set; }
        public required CategoryDTO UpdatedCategory { get; set; }
        public required List<ShoppingListDTO> UpdatedShoppingLists { get; set; }

    }
}
