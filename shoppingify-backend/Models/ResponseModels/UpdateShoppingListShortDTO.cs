namespace shoppingify_backend.Models.ResponseModels
{
    public class UpdateShoppingListShortDTO
    {
        public required string Message { get; set; }
        public required ShoppingListDTO UpdatedShoppingList { get; set; }
    }
}
