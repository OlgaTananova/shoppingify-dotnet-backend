using shoppingify_backend.Models.ResponseModels;

namespace shoppingify_backend.Helpers
{
    public static class MockGPTResponseGenerator
    {
        public static GPTShoppingListDTO GenerateResponse()
        {
            GPTShoppingListDTO result = new GPTShoppingListDTO
            {
                Items = new List<GPTShoppingListItemDTO>(),
                DateOfPurchase = "January 20, 2024",
                SalesTax = 0
            };
            result.Items.Add(new GPTShoppingListItemDTO
            {
                ItemName = "St Pierre Sesame Seed Brioche Burger Buns",
                ItemUnits = "pcs",
                ItemQuantity = 10,
                ItemPricePerUnit = 4.99M,
                ItemPrice = 49.9M
            });
            result.Items.Add(new GPTShoppingListItemDTO
            {
                ItemName = "Magnum Double Chocolate Ice Cream Bars",
                ItemPrice = 62.9M,
                ItemPricePerUnit = 6.29M,
                ItemQuantity = 10,
                ItemUnits = "pcs"
            });
            result.Items.Add(new GPTShoppingListItemDTO
            {
                ItemName = "Kobe Ground Beef Patties",
                ItemPrice = 109.9M,
                ItemPricePerUnit = 10.99M,
                ItemQuantity = 10,
                ItemUnits = "pcs",
            });

            return result;
        }
    }
}