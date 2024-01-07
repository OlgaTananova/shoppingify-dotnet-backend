namespace shoppingify_backend.Models.ValidationModels
{
    public class ChangeItemPricePerUnitModel
    {
        public required string ShoppingListId { get; set; }
        public required string ItemId { get; set; }
    }
}
