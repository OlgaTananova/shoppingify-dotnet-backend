using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;
using System.Diagnostics.SymbolStore;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace shoppingify_backend.Services
{
    public interface IShoppingListService
    {
        Task<ShoppingListDTO> CreateShoppingList(ShoppingListModel shoppingListData);
        Task<List<ShoppingListDTO>> GetShoppingLists();

        Task<object> DeleteShoppingList(string slId);
    }
    public class ShoppingListService : IShoppingListService
    {
        private readonly ApplicationContext _context;
        private readonly IUserResolverService _userResolverService;

        public ShoppingListService(ApplicationContext context, IUserResolverService userResolverService)
        {
            _context = context;
            _userResolverService = userResolverService;
        }
        public async Task<ShoppingListDTO> CreateShoppingList(ShoppingListModel shoppingListData)
        {
            string userId = _userResolverService.GetCurrentUserId();

            // If there is the active shopping list - throw exception
            var activeShL = _context.ShoppingLists.FirstOrDefaultAsync(sl => sl.Status == ShoppingListStatus.Active);
          
            if (activeShL.Result != null)
            {
                throw new BadRequestException("The active shopping list already exists.");
            }
            // Parser ids
            bool parsedUserId = Guid.TryParse(userId, out var userIdGuid);
            bool parsedItemId = Guid.TryParse(shoppingListData.ItemId, out var itemIdGuid);
            bool parsedCategoryId = Guid.TryParse(shoppingListData.CategoryId, out var categoryIdGuid);

            if (!parsedCategoryId || !parsedItemId || !parsedUserId)
            {
                throw new BadHttpRequestException("Failed to parse userId or/and itemId or/and categoryId");
            }

            //Find the category and item

            var addedCategory = await _context.Categories.FindAsync(categoryIdGuid);
            if (addedCategory == null)
            {
                throw new NotFoundException($"Cannot find the category with {shoppingListData.CategoryId}");
            }

            var addedItem = await _context.Items.FindAsync(itemIdGuid);
            if (addedItem == null)
            {
                throw new NotFoundException($"Cannot find the item with {shoppingListData.ItemId},");
            }
            
            // Create a new shopping list and shopping list item
            ShoppingList newShL = new ShoppingList
            {
                OwnerId = userIdGuid,
            };

            ShoppingListItem newShLI = new ShoppingListItem
            {
                ShoppingListId = newShL.Id,
                ShoppingList = newShL,
                Item = addedItem,
                Category = addedCategory,
                CategoryId = categoryIdGuid,
                ItemId = itemIdGuid,
                OwnerId = userIdGuid
            };

            newShL.ShoppingListItems.Add(newShLI);
            _context.ShoppingLists.Add(newShL);
            var successfullyCreated = await _context.SaveChangesAsync();

            if (successfullyCreated <= 0)
            {
                throw new BadRequestException("Failed to create a new shopping list and add the item.");
            }

            List<ShoppingListItemDTO> itemsInShL = new List<ShoppingListItemDTO>
            {
                new ShoppingListItemDTO
                {
                    ItemId = newShLI.ItemId.ToString(),
                    CategoryId = newShLI.CategoryId.ToString(),
                    Quantity = newShLI.Quantity,
                    Status = newShLI.Status.ToString().ToLower(),
                    Units = newShLI.Units,
                    PricePerUnit = newShLI.PricePerUnit,
                    Price = newShLI.Price
                }
            };

            var result = new ShoppingListDTO
            {
                _id = newShL.Id.ToString(),
                Heading = newShL.Heading,
                Date = newShL.Date.ToLongDateString(),
                Owner = newShL.OwnerId.ToString(),
                Status = newShL.Status.ToString().ToLower(),
                SalesTax = newShL.SalesTax,
                Items = itemsInShL
            };

            return result;
        }
        public Task<List<ShoppingListDTO>> GetShoppingLists()
        {
            string userId = _userResolverService.GetCurrentUserId();

           var result = _context.ShoppingLists.Include(i => i.ShoppingListItems).Select(sl => new ShoppingListDTO
           {
               _id = sl.Id.ToString().ToLower(),
               Heading = sl.Heading,
               Date = sl.Date.ToLongDateString(),
               Owner = sl.OwnerId.ToString().ToLower(),
               Status = sl.Status.ToString().ToLower(),
               SalesTax = sl.SalesTax,
               Items = sl.ShoppingListItems.Select(sli => new ShoppingListItemDTO {
                   ItemId = sli.ItemId.ToString().ToLower(),
                   CategoryId = sli.CategoryId.ToString().ToLower(),
                   Quantity = sli.Quantity,
                   Status = sli.Status.ToString().ToLower(),
                   Units = sli.Units,
                   PricePerUnit = sli.PricePerUnit,
                   Price = sli.Price
               }).ToList()
           }).ToListAsync();

            return result;

        }

        public async Task<object> DeleteShoppingList(string slId)
        {
            string userId = _userResolverService.GetCurrentUserId();

            if (!Guid.TryParse(slId, out Guid slIdGuid))
            {
                throw new BadRequestException("Failed to parse the shopping list id.");
            }

            var deletedShL = await _context.ShoppingLists.Include(sl => sl.ShoppingListItems).FirstOrDefaultAsync(s => s.Id == slIdGuid);
            
            if (deletedShL == null)
            {
                throw new NotFoundException($"Failed to find the shopping list with {slId} id.");
            }

            deletedShL.IsDeleted = true;
            deletedShL.Status = ShoppingListStatus.Deleted;

            foreach(var sli in deletedShL.ShoppingListItems)
            {
                sli.IsDeleted = true;
            }
            _context.ShoppingLists.Update(deletedShL);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return (new { message = "The shopping list was successfully deleted." });
            }
            throw new BadRequestException($"Failed to delete the shopping list with {slId} id.");
        }

    }
}