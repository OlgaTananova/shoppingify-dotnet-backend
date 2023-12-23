using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace shoppingify_backend.Services
{
    public interface IShoppingListService
    {
        Task<ShoppingListDTO> CreateShoppingList(ShoppingListModel shoppingListData);
        Task<List<ShoppingListDTO>> GetShoppingLists();
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

            bool parsedUserId = Guid.TryParse(userId, out var userIdGuid);
            bool parsedItemId = Guid.TryParse(shoppingListData.ItemId, out var itemIdGuid);
            bool parsedCategoryId = Guid.TryParse(shoppingListData.CategoryId, out var categoryIdGuid);

            if (!parsedCategoryId || !parsedItemId || !parsedUserId)
            {
                throw new BadHttpRequestException("Failed to parse userId or/and itemId or/and categoryId");
            }

            var addedCategory = await _context.Categories.FindAsync(categoryIdGuid);

            if (addedCategory == null)
            {
                throw new NotFoundException($"Cannot find the category with {shoppingListData.CategoryId}");
            }

            ShoppingList newShL = new ShoppingList
            {
                OwnerId = userIdGuid,
            };

            ShoppingListItem newShLI = new ShoppingListItem
            {
                ShoppingListId = newShL.Id,
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

    }
}