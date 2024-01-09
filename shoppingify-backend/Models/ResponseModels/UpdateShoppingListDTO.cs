namespace shoppingify_backend.Models.ResponseModels
{
    public class UpdateShoppingListDTO
    {
        public required string Message { get; set; }
        public required ShoppingListDTO UpdatedShoppingList { get; set; }
    }
}
