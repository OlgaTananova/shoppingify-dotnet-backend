using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;

namespace shoppingify_backend.Services
{
    public interface IItemService
    {
        Task<List<ItemDTO>> GetAllItems();

        Task<ItemDTO> GetItem(string id);
        Task<object> CreateItem(ItemModel item);
        Task<object> UpdateItem(string id, ItemModel item);
        Task<object> DeleteItem(string id);

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
            var result = await _context.Items
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

        public async Task<object> CreateItem(ItemModel item)
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
                var responseResult = new
                {
                    item = new ItemDTO
                    {
                        _id = newItem.Id.ToString(),
                        Name = newItem.ItemName,
                        CategoryId = newItem.CategoryId.ToString(),
                        Owner = newItem.OwnerId.ToString(),
                        Note = newItem.Note,
                        Image = newItem.Image,
                        IsDeleted = newItem.IsDeleted
                    },
                    category = new CategoryDTO
                    {
                        _id = updatedCategory.Id.ToString(),
                        Category = updatedCategory.CategoryName,
                        Owner = updatedCategory.OwnerId.ToString(),
                        Items = updatedCategory.Items.Select(i => i.Id.ToString()).ToList()
                    }

                };
                return responseResult;
            }
            throw new BadRequestException("Failed to create a new item.");
        }

        public async Task<object> UpdateItem(string id, ItemModel item)
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
            //Find the current category and a new category

            Category oldCategory = updatedItem.Category;
            Category? newCategory;

            if (categoryIdGuid != updatedItem.CategoryId)
            {
                newCategory = await _context.Categories.Include(c => c.Items).FirstOrDefaultAsync(cat => cat.Id == categoryIdGuid);

                if (newCategory == null)
                {
                    throw new NotFoundException($"The category with id {updatedItem.CategoryId}, {categoryIdGuid} was not found.");
                }
            }
            else
            {
                newCategory = oldCategory;
            }

            //Update item
            updatedItem.ItemName = item.Name;
            updatedItem.CategoryId = categoryIdGuid;
            updatedItem.Note = item.Note;
            updatedItem.Image = item.Image;

            _context.Items.Update(updatedItem);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var responseResult = new
                {
                    updatedItem = new ItemDTO
                    {
                        _id = updatedItem.Id.ToString(),
                        Name = updatedItem.ItemName,
                        CategoryId = updatedItem.CategoryId.ToString(),
                        Note = updatedItem.Note,
                        Owner = updatedItem.OwnerId.ToString(),
                        Image = updatedItem.Image,
                        IsDeleted = updatedItem.IsDeleted
                    },
                    deleteFromCategory = new CategoryDTO
                    {
                        _id = oldCategory.Id.ToString().ToLower(),
                        Category = oldCategory.CategoryName,
                        Owner = oldCategory.OwnerId.ToString().ToLower(),
                        Items = oldCategory.Items.Select(i => i.Id.ToString().ToLower()).ToList()
                    },
                    addToCategory = new CategoryDTO
                    {
                        _id = newCategory.Id.ToString().ToLower(),
                        Category = newCategory.CategoryName,
                        Owner = newCategory.OwnerId.ToString().ToLower(),
                        Items = newCategory.Items.Select(i => i.Id.ToString().ToLower()).ToList()
                    }
                };
                return responseResult;
            }
            throw new BadRequestException("Failed to update the item.");
        }
        public async Task<object> DeleteItem(string id)
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

            deletedItem.ItemName = $"{deletedItem.ItemName} - Deleted";
            deletedItem.IsDeleted = true;

            _context.Items.Update(deletedItem);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                var responseResult = new
                {
                    item = new ItemDTO
                    {
                        _id = deletedItem.Id.ToString(),
                        Name = deletedItem.ItemName,
                        CategoryId = deletedItem.CategoryId.ToString(),
                        Owner = deletedItem.OwnerId.ToString(),
                        Image = deletedItem.Image,
                        Note = deletedItem.Note,
                        IsDeleted = deletedItem.IsDeleted
                    },
                    updatedCategory = new CategoryDTO
                    {
                        _id = uptadedCategory.Id.ToString(),
                        Owner = uptadedCategory.OwnerId.ToString(),
                        Category = uptadedCategory.CategoryName,
                        Items = uptadedCategory.Items.Select(i => i.Id.ToString()).ToList()
                    }
                };
                return responseResult;
            }
            throw new BadRequestException($"Failed to delete the item with {id} id.");
        }
    }
}

