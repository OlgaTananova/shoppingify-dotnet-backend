namespace shoppingify_backend.Models.ResponseModels
{
    public class AddShoppingListDTO
    {
        public required string Message { get; set; }
        public required ShoppingListDTO AddedShoppingList { get; set; }
    }
}
