using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;
using shoppingify_backend.Models.Entities;
using shoppingify_backend.Models.ResponseModels;
using shoppingify_backend.Models.ValidationModels;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace shoppingify_backend.Services
{
    public interface IItemService
    {
        Task<List<ItemDTO>> GetAllItems();

        Task<ItemDTO> GetItem(string id);
        Task<CreateItemDTO> CreateItem(ItemModel item);
        Task<UpdateItemDTO> UpdateItem(string id, ItemModel item);
        Task<DeleteItemDTO> DeleteItem(string id);

    }
    public class ItemService : IItemService
    {
        private readonly ApplicationContext _context;
        private readonly IUserResolverService _userResolverService;
        public ItemService(ApplicationContext context, IUserResolverService userResolverService)
        {
            _context = context;
            _userResolverService = userResolverService;
        }

        public async Task<List<ItemDTO>> GetAllItems()
        {
            var result = await _context.Items.Where(i => i.IsDeleted == false)
                .ToListAsync();

            if (result.Count > 0)
            {
                var responseResult = result.Select(i => new ItemDTO
                {
                    _id = i.Id.ToString(),
                    CategoryId = i.CategoryId.ToString(),
                    Name = i.ItemName,
                    Owner = i.OwnerId.ToString(),
                    Note = i.Note,
                    Image = i.Image,
                    IsDeleted = i.IsDeleted
                }).ToList();
                return responseResult;
            }
            return new List<ItemDTO>();
        }

        public async Task<ItemDTO> GetItem(string id)
        {
            if (!Guid.TryParse(id, out var guidId))
            {
                throw new BadRequestException("Failed to read the item id.");
            }
            var result = await _context.Items.FindAsync(guidId);

            if (result != null)
            {
                var responseResult = new ItemDTO
                {
                    _id = result.Id.ToString(),
                    Name = result.ItemName,
                    CategoryId = result.CategoryId.ToString(),
                    Owner = result.OwnerId.ToString(),
                    Note = result.Note,
                    Image = result.Image,
                    IsDeleted = result.IsDeleted
                };
                return responseResult;
            }
            throw new NotFoundException($"Item with id {id} was not found.");
        }

        public async Task<CreateItemDTO> CreateItem(ItemModel item)
        {
            var userId = _userResolverService.GetCurrentUserId();

            if (!Guid.TryParse(userId, out var userIdGuid))
            {
                throw new BadRequestException("Unable to read the user id.");
            }

            if (!Guid.TryParse(item.CategoryId, out var categoryIdGuid))
            {
                throw new BadRequestException("Unable to read the category id.");
            }

            // Find the categoty in the database
            var updatedCategory = await _context.Categories.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == categoryIdGuid);

            if (updatedCategory == null)
            {
                throw new NotFoundException("Unable to find the category.");
            }

            // Create a new item
            var newItem = new Item
            {
                ItemName = item.Name,
                Category = updatedCategory,
                CategoryId = categoryIdGuid,
                OwnerId = userIdGuid,
                Image = item.Image,
                Note = item.Note,
                IsDeleted = false
            };

            // Add item to the batabase and add it to the category
            _context.Items.Add(newItem);
            updatedCategory.Items.Add(newItem);

            var result = await _context.SaveChangesAsync();

            //Check the result

            if (result > 0)
            {

                // Retrieve all items belong to the updatedCategory    
                var responseResult = new CreateItemDTO
                {
                    Item = new ItemDTO
                    {
                        _id = newItem.Id.ToString().ToLower(),
                        Name = newItem.ItemName,
                        CategoryId = newItem.CategoryId.ToString().ToLower(),
                        Owner = newItem.OwnerId.ToString().ToLower(),
                        Note = newItem.Note,
                        Image = newItem.Image,
                        IsDeleted = newItem.IsDeleted
                    },
                    Category = new CategoryDTO
                    {
                        _id = updatedCategory.Id.ToString().ToLower(),
                        Category = updatedCategory.CategoryName,
                        Owner = updatedCategory.OwnerId.ToString().ToLower(),
                        Items = updatedCategory.Items.Where(i => i.IsDeleted == false).Select(i => i.Id.ToString()).ToList()
                    }

                };
                return responseResult;
            }
            throw new BadRequestException("Failed to create a new item.");
        }

        public async Task<UpdateItemDTO> UpdateItem(string id, ItemModel item)
        {
            // Parse Ids
            if (!Guid.TryParse(id, out var idGuid) || !Guid.TryParse(item.CategoryId, out var categoryIdGuid))
            {
                throw new BadRequestException("Not valid the item's Id or the category's Id.");
            }

            //Find the updated item
            var updatedItem = await _context.Items.Include(i => i.Category).Include(i => i.ShoppingListItems).FirstOrDefaultAsync(item => item.Id == idGuid);

            if (updatedItem == null)
            {
                throw new NotFoundException($"The item with id {id} was not found.");
            }

            //Find the updated category
            var updatedCategory = await _context.Categories.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == categoryIdGuid);

            if (updatedCategory == null)
            {
                throw new NotFoundException($"The item with {item.CategoryId} was not found.");
            }


            //Update item
            updatedItem.ItemName = item.Name;
            updatedItem.CategoryId = categoryIdGuid;
            updatedItem.Note = item.Note;
            updatedItem.Image = item.Image;

            //Update category Id in all shoppinglist items belong to the updated item
            foreach (var sli in updatedItem.ShoppingListItems)
            {
                sli.CategoryId = updatedItem.CategoryId;
                _context.ShoppingListItems.Update(sli);
            }

            _context.Items.Update(updatedItem);

            var result = await _context.SaveChangesAsync();

            // Get all non-deleted categories including those that have the updated item
            var updatedCategories = await _context.Categories.Include(c => c.Items).Select(c => new CategoryDTO
            {
                _id = c.Id.ToString().ToLower(),
                Owner = c.OwnerId.ToString().ToLower(),
                Category = c.CategoryName,
                Items = c.Items.Where(i => i.IsDeleted == false).Select(i => i.Id.ToString().ToLower()).ToList(),
            }).ToListAsync();

            // Get all non-deleted shopping lists including those that have the updated item
            var updatedShoppingLists = await _context.ShoppingLists.Where(sl => sl.IsDeleted == false).Include(sl => sl.ShoppingListItems).Select(sl => MappingHandler.MapToShoppingListDTO(sl)).ToListAsync();

            if (result > 0)
            {
                var responseResult = new UpdateItemDTO
                {
                    UpdatedItem = new ItemDTO
                    {
                        _id = updatedItem.Id.ToString(),
                        Name = updatedItem.ItemName,
                        CategoryId = updatedItem.CategoryId.ToString(),
                        Note = updatedItem.Note,
                        Owner = updatedItem.OwnerId.ToString(),
                        Image = updatedItem.Image,
                        IsDeleted = updatedItem.IsDeleted
                    },
                    UpdatedCategories = updatedCategories.Count > 0 ? updatedCategories : new List<CategoryDTO>(),
                    UpdatedShoppingLists = updatedShoppingLists.Count > 0 ? updatedShoppingLists : new List<ShoppingListDTO>()
                };
                return responseResult;
            }
            throw new BadRequestException("Failed to update the item.");
        }
        public async Task<DeleteItemDTO> DeleteItem(string id)
        {
            // Parse id to Guid
            if (!Guid.TryParse(id, out var guidId))
            {
                throw new BadRequestException("Cannot parse the item's id.");
            }

            // Find the deleted item
            var deletedItem = await _context.Items.Include(i => i.Category).Include(i => i.ShoppingListItems).FirstOrDefaultAsync(item => item.Id == guidId);

            if (deletedItem == null)
            {
                throw new NotFoundException($"Failed to find the item with {id} id.");
            }

            var uptadedCategory = deletedItem.Category;

            // Mark the item as deleted
            // Update shoppingListItems related to this item and mark them deleted

            deletedItem.ItemName = $"{deletedItem.ItemName} - Deleted";
            deletedItem.IsDeleted = true;
            foreach (var sli in deletedItem.ShoppingListItems)
            {
                //sli.IsDeleted = true;
                _context.ShoppingListItems.Update(sli);
            }

            _context.Items.Update(deletedItem);
            var result = await _context.SaveChangesAsync();

            // Get the updated shopping lists
            var updatedShoppingLists = await _context.ShoppingLists.Where(sl => sl.IsDeleted == false).Include(sl => sl.ShoppingListItems).Select(sl => MappingHandler.MapToShoppingListDTO(sl)).ToListAsync();

            if (result > 0)
            {
                var responseResult = new DeleteItemDTO
                {
                    Message = "The item was successfully deleted.",
                    DeletedItem = new ItemDTO
                    {
                        _id = deletedItem.Id.ToString(),
                        Name = deletedItem.ItemName,
                        CategoryId = deletedItem.CategoryId.ToString(),
                        Owner = deletedItem.OwnerId.ToString(),
                        Image = deletedItem.Image,
                        Note = deletedItem.Note,
                        IsDeleted = deletedItem.IsDeleted
                    },
                    UpdatedCategory = new CategoryDTO
                    {
                        _id = uptadedCategory.Id.ToString(),
                        Owner = uptadedCategory.OwnerId.ToString(),
                        Category = uptadedCategory.CategoryName,
                        Items = uptadedCategory.Items.Where(i => i.IsDeleted == false).Select(i => i.Id.ToString()).ToList()
                    },
                    UpdatedShoppingLists = updatedShoppingLists.Count > 0 ? updatedShoppingLists : new List<ShoppingListDTO>(),
                };
                return responseResult;
            }
            throw new BadRequestException($"Failed to delete the item with {id} id.");
        }
    }
}

