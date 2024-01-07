using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;
using shoppingify_backend.Models.Entities;
using shoppingify_backend.Models.ResponseModels;
using shoppingify_backend.Models.ValidationModels;

namespace shoppingify_backend.Services
{
    public interface IShoppingListItemService
    {
        public Task<UpdateShoppingListShortDTO> AddItemToShoppingList(AddItemToShoppingListModel addItemToSL);
        //public Task<UpdateShoppingListDTO> DeleteItemFromShoppingList(DeleteItemFromShoppingListModel deleteItemFromSL);
        //public Task<UpdateShoppingListDTO> ChangeItemQuantity(ChangeItemQuantityModel changeItemQtyModel);
        //public Task<UpdateShoppingListDTO> ChangeItemStatus(ChangeItemStatusModel changeItemStatusModel);
        //public Task<UpdateShoppingListDTO> ChangeItemUnits(ChangeItemUnitsModel changeItemUnitsModel);

        // public Task<UpdateShoppingListDTO> ChangeItemPrice(ChangeItemPriceModel changeItemPriceModel);
        //public Task<UpdateShoppingListDTO> ChangeItemPricePerUnit();

    }
    public class ShoppingListItemService : IShoppingListItemService
    {
        private readonly ApplicationContext _context;
        private readonly IUserResolverService _userResolverService;

        public ShoppingListItemService(ApplicationContext context, IUserResolverService userResolverService)
        {
            _context = context;
            _userResolverService = userResolverService;
        }
        public async Task<UpdateShoppingListShortDTO> AddItemToShoppingList(AddItemToShoppingListModel addItemToSl)
        {
            string userId = _userResolverService.GetCurrentUserId();

            // Parse id
            if (!Guid.TryParse(addItemToSl.ShoppingListId, out Guid guidSlId) 
                || !Guid.TryParse(addItemToSl.ItemId, out Guid guidItemId) 
                || !Guid.TryParse(addItemToSl.CategoryId, out Guid guidCategoryId)
                || !Guid.TryParse(userId, out Guid guidUserId))
            {
                throw new BadRequestException("Cannot parse the shopping list's id or item's id or category's id or the user's id.");

            }

            // Find the shopping list by id and update it heading
            var updatedSL = await _context.ShoppingLists.Include(sl => sl.ShoppingListItems).FirstOrDefaultAsync(sl => sl.Id == guidSlId && sl.Status == ShoppingListStatus.Active);
            var addedItem = await _context.Items.FindAsync(guidItemId);
            var addedCategory = await _context.Categories.FindAsync(guidCategoryId);
            if (updatedSL == null || addedItem == null || addedCategory == null)
            {
                throw new NotFoundException($"The shopping list with {addItemToSl.ShoppingListId} id, or the item with {addItemToSl.ItemId} id, or the category with {addItemToSl.CategoryId} id was not found.");
            }

            ShoppingListItem newShoppingListItem = new ShoppingListItem
            {
                ItemId = guidItemId,
                CategoryId = guidCategoryId,
                ShoppingListId = guidSlId,
                ShoppingList = updatedSL,
                Category = addedCategory,
                Item = addedItem,
                OwnerId = guidUserId
            };

            updatedSL.ShoppingListItems.Add(newShoppingListItem);
            var result = await _context.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadImageFormatException("Failed to add a new item to the shopping list");
            }

            return new UpdateShoppingListShortDTO
            {
                Message = "The item was successfully added to the shopping list",
                UpdatedShoppingList = new ShoppingListDTO
                {
                    _id = updatedSL.Id.ToString().ToLower(),
                    Heading = updatedSL.Heading,
                    Date = updatedSL.Date.ToLongDateString(),
                    Owner = updatedSL.OwnerId.ToString().ToLower(),
                    Status = updatedSL.Status.ToString().ToLower(),
                    SalesTax = updatedSL.SalesTax,
                    Items = updatedSL.ShoppingListItems.Select(sli => new ShoppingListItemDTO
                    {
                        _id = sli.Id.ToString().ToLower(),
                        ItemId = sli.ItemId.ToString().ToLower(),
                        CategoryId = sli.CategoryId.ToString().ToLower(),
                        Units = sli.Units,
                        Status = sli.Status.ToString().ToLower(),
                        Quantity = sli.Quantity,
                        PricePerUnit = sli.PricePerUnit,
                        Price = sli.Price,
                        IsDeleted = sli.IsDeleted,
                    }).ToList()
                }
            };
        }
    }
}
