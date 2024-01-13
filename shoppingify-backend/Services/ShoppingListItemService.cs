using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;
using shoppingify_backend.Models.Entities;
using shoppingify_backend.Models.ResponseModels;
using shoppingify_backend.Models.ValidationModels;

namespace shoppingify_backend.Services
{
    public interface IShoppingListItemService
    {
        public Task<UpdateShoppingListDTO> AddItemToShoppingList(AddItemToShoppingListModel addItemToSL);
        public Task<UpdateShoppingListDTO> DeleteItemFromShoppingList(DeleteItemFromShoppingListModel deleteItemFromSL);
        public Task<UpdateShoppingListDTO> ChangeItemQuantity(ChangeItemQuantityModel changeItemQtyModel);
        public Task<UpdateShoppingListDTO> ChangeItemStatus(ChangeItemStatusModel changeItemStatusModel);
        public Task<UpdateShoppingListDTO> ChangeItemUnits(ChangeItemUnitsModel changeItemUnitsModel);
        public Task<UpdateShoppingListDTO> ChangeItemPrice(ChangeItemPriceModel changeItemPriceModel);
        public Task<UpdateShoppingListDTO> ChangeItemPricePerUnit(ChangeItemPricePerUnitModel changeItemPricePerUnit);
        public Task<(ShoppingList, ShoppingListItem)> FindShoppingListAndShoppingListItem(string shoppingListId, string shoppingListItemId);

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

        public async Task<(ShoppingList, ShoppingListItem)> FindShoppingListAndShoppingListItem(string shoppingListId, string shoppingListItemId)
        {

            // Parse the shopping list's id and the shopping list item's id
            if (!Guid.TryParse(shoppingListId, out Guid guidSLId) || !Guid.TryParse(shoppingListItemId, out Guid guidSLIId))
            {
                throw new BadHttpRequestException("Cannot parse the shopping list's id and/or shopping item's id");
            }

            // Find the updated shopping list
            var updatedShoppingList = await _context.ShoppingLists.Include(sl => sl.ShoppingListItems).FirstOrDefaultAsync(sl => sl.Id == guidSLId && sl.Status == ShoppingListStatus.Active);
            if (updatedShoppingList == null)
            {
                throw new NotFoundException($"The shopping list with {shoppingListId} id was not found.");
            }

            // Find the updated shopping list item
            var updatedShoppingListItem = await _context.ShoppingListItems.FindAsync(guidSLIId);
            if (updatedShoppingListItem == null)
            {
                throw new NotFoundException($"The shopping item with {shoppingListItemId} id was not found.");
            }
            return (updatedShoppingList, updatedShoppingListItem);
        }
        public async Task<UpdateShoppingListDTO> AddItemToShoppingList(AddItemToShoppingListModel addItemToSl)
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

            // Find the shopping list by id and update its heading
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

            return new UpdateShoppingListDTO
            {
                Message = "The item was successfully added to the shopping list",
                UpdatedShoppingList = MappingHandler.MapToShoppingListDTO(updatedSL),
            };
        }
        public async Task<UpdateShoppingListDTO> ChangeItemQuantity(ChangeItemQuantityModel changeItemQuantity)
        {
            (ShoppingList updatedShoppingList, ShoppingListItem updatedShoppingListItem)  = await FindShoppingListAndShoppingListItem(changeItemQuantity.ShoppingListId, changeItemQuantity.ShoppingListItemId);
            
            // Update the shopping list item's quantity, update its price, and save the changes

            updatedShoppingListItem.Quantity = changeItemQuantity.Quantity;
            updatedShoppingListItem.Price = updatedShoppingListItem.PricePerUnit * changeItemQuantity.Quantity;
            _context.ShoppingListItems.Update(updatedShoppingListItem);

            var result = await _context.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("Failed to update the shopping item's quantity");
            }

            return new UpdateShoppingListDTO
            {
                Message = "The shopping item's quantity was successfully updated.",
                UpdatedShoppingList = MappingHandler.MapToShoppingListDTO(updatedShoppingList),
            };

        }
        public async Task<UpdateShoppingListDTO> ChangeItemStatus(ChangeItemStatusModel changeItemStatus)
        {
            (ShoppingList updatedShoppingList, ShoppingListItem updatedShoppingListItem) = await FindShoppingListAndShoppingListItem(changeItemStatus.ShoppingListId, changeItemStatus.ShoppingListItemId);

            // Validate status
            string capitalizedFirstLetterStatus = changeItemStatus.Status.Substring(0, 1).ToUpper() + changeItemStatus.Status.Substring(1);

            if (!Enum.TryParse(capitalizedFirstLetterStatus, out ItemStatus status))
            {
                throw new BadRequestException("You tried to set the wrong status for the shopping item.");
            }

            // Change the shopping list item's status and save the changes

            updatedShoppingListItem.Status = status;
            _context.ShoppingListItems.Update(updatedShoppingListItem);
            var result = await _context.SaveChangesAsync();
            if (result <= 0)
            {
                throw new BadRequestException("Failed to update the shopping item's status.");
            }

            return new UpdateShoppingListDTO
            {
                Message = "The shopping item's status was successfully updated.",
                UpdatedShoppingList = MappingHandler.MapToShoppingListDTO(updatedShoppingList),
            };
        }
        public async Task<UpdateShoppingListDTO> ChangeItemUnits(ChangeItemUnitsModel changeItemUnits)
        {
            (ShoppingList updatedShoppingList, ShoppingListItem updatedShoppingListItem) = await FindShoppingListAndShoppingListItem(changeItemUnits.ShoppingListId, changeItemUnits.ShoppingListItemId);

            // Update the shopping list item's units
            updatedShoppingListItem.Units = changeItemUnits.Units;
            _context.ShoppingListItems.Update(updatedShoppingListItem);
            var result = await _context.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("Failed to update the shopping item's units.");
            }

            return new UpdateShoppingListDTO
            {
                Message = "The shopping item's units were successfully updated.",
                UpdatedShoppingList = MappingHandler.MapToShoppingListDTO(updatedShoppingList),
            };
        }
        public async Task<UpdateShoppingListDTO> ChangeItemPricePerUnit(ChangeItemPricePerUnitModel changeItemPricePerUnit)
        {
            (ShoppingList updatedShoppingList, ShoppingListItem updatedShoppingListItem) = await FindShoppingListAndShoppingListItem(changeItemPricePerUnit.ShoppingListId, changeItemPricePerUnit.ShoppingListItemId);

            // Update shopping list item's price per a unit and save the changes

            updatedShoppingListItem.PricePerUnit = changeItemPricePerUnit.PricePerUnit;
            updatedShoppingListItem.Price = updatedShoppingListItem.Quantity * changeItemPricePerUnit.PricePerUnit;
            _context.ShoppingListItems.Update(updatedShoppingListItem);

            var result = await _context.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("Failed to update the shopping item's price per a unit.");
            }

            return new UpdateShoppingListDTO
            {
                Message = "The shopping item's price per a unit was successfully updated.",
                UpdatedShoppingList = MappingHandler.MapToShoppingListDTO(updatedShoppingList),
            };
        }
        public async Task<UpdateShoppingListDTO> ChangeItemPrice(ChangeItemPriceModel changeItemPrice)
        {
            (ShoppingList updatedShoppingList, ShoppingListItem updatedShoppingListItem) = await FindShoppingListAndShoppingListItem(changeItemPrice.ShoppingListId, changeItemPrice.ShoppingListItemId);

            // Update the shopping list item's price, price per a unit, and save the changes
            updatedShoppingListItem.Price = changeItemPrice.Price;
            updatedShoppingListItem.PricePerUnit = changeItemPrice.Price / updatedShoppingListItem.Quantity;
            _context.ShoppingListItems.Update(updatedShoppingListItem);
            var result = await _context.SaveChangesAsync();
            if (result <= 0)
            {
                throw new BadRequestException("Failed to update the shopping item's price.");
            }

            return new UpdateShoppingListDTO
            {
                Message = "The shopping item's price was successfully updated.",
                UpdatedShoppingList = MappingHandler.MapToShoppingListDTO(updatedShoppingList),
            };
        }
        public async Task<UpdateShoppingListDTO> DeleteItemFromShoppingList(DeleteItemFromShoppingListModel deleteItemFromShoppingList)
        {
            (ShoppingList updatedShoppingList, ShoppingListItem deletedShoppingListItem) = await FindShoppingListAndShoppingListItem(deleteItemFromShoppingList.ShoppingListId, deleteItemFromShoppingList.ShoppingListItemId);

            // Mark the shopping list item deleted and save the changes
            deletedShoppingListItem.IsDeleted = true;
            _context.ShoppingListItems.Update(deletedShoppingListItem);

            var result = await _context.SaveChangesAsync();
            if (result <= 0)
            {
                throw new BadRequestException("Failed to delete the shopping item.");
            }

            return new UpdateShoppingListDTO
            {
                Message = "The shopping item was successfully deleted.",
                UpdatedShoppingList = MappingHandler.MapToShoppingListDTO(updatedShoppingList),
            };
        }
    }
}
 