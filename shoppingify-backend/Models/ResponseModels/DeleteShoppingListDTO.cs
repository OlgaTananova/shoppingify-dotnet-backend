namespace shoppingify_backend.Models.ResponseModels
{
    public class DeleteShoppingListDTO
    {
        public required string Message { get; set; }
        public required ShoppingListDTO DeletedShoppingList { get; set; }
        public required List<ShoppingListDTO> RemainingShoppingLists { get; set; }
    }
}
