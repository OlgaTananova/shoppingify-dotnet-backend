using shoppingify_backend.Models.Entities;
using shoppingify_backend.Models.ResponseModels;

namespace shoppingify_backend.Helpers
{
    public static class MappingHandler
    {
        public static ShoppingListDTO MapToShoppingListDTO(ShoppingList sl)
        {
            return new ShoppingListDTO
            {
                _id = sl.Id.ToString().ToLower(),
                Heading = sl.Heading,
                Owner = sl.OwnerId.ToString().ToLower(),
                Status = sl.Status.ToString().ToLower(),
                SalesTax = sl.SalesTax,
                Date = sl.Date.ToLongDateString(),
                IsDeleted = sl.IsDeleted,
                Items = sl.ShoppingListItems.Where(sli => !sli.IsDeleted).Select(MappingHandler.MapToShoppingListItemDTO).ToList(),
            };
        }

        public static ShoppingListItemDTO MapToShoppingListItemDTO(ShoppingListItem sli)
        {
            return new ShoppingListItemDTO
            {
                _id = sli.Id.ToString().ToLower(),
                CategoryId = sli.CategoryId.ToString().ToLower(),
                Units = sli.Units,
                Status = sli.Status.ToString(),
                Quantity = sli.Quantity,
                PricePerUnit = sli.PricePerUnit,
                Price = sli.Price,
                ItemId = sli.ItemId.ToString().ToLower(),
                IsDeleted = sli.IsDeleted,
            };
        }

    }
}
