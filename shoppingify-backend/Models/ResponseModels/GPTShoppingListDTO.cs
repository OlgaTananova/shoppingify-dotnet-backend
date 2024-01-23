namespace shoppingify_backend.Models.ResponseModels
{
    public class GPTShoppingListDTO
    {
        public required List<GPTShoppingListItemDTO> Items { get; set; }
        public required decimal SalesTax { get; set; }
        public required string DateOfPurchase { get; set; }

    }

    public class GPTShoppingListItemDTO
    {
        public required string ItemName { get; set; }
        public required string ItemUnits { get; set; }
        public required decimal ItemQuantity  { get; set; }
        public required decimal ItemPricePerUnit { get; set; }
        public required decimal ItemPrice { get; set; }
    }
}
