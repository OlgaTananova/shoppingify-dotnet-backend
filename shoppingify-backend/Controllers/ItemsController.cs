using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;
using shoppingify_backend.Services;

namespace shoppingify_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly string _userId;
        private readonly IUserResolverService _userResolverService;

        public ItemsController(ApplicationContext context, IUserResolverService userResolverService)
        {
            _context = context;
            _userResolverService = userResolverService;
            _userId = userResolverService.GetCurrentUserId();
        }

        [HttpGet]
        [Authorize]

        public async Task<IActionResult> GetAllItems()
        {
            var result = await _context.Items.Where(i=> i.IsDeleted == false).ToListAsync();

            if (result.Count > 0)
            {
                var responseResult = result.Select(i => new ItemDTO
                {
                    _id = i.Id.ToString(),
                    CategoryId = i.CategoryId.ToString(),
                    Name = i.ItemName,
                    Owner = i.OwnerId.ToString(),
                    Note = i.Note,
                    Image = i.Image
                }).ToList();
                return Ok(responseResult);
            }
            return Ok(new List<object>());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetItem(string id)
        {
            if (!Guid.TryParse(id, out var guidId))
            {
                throw new BadRequestException("Could not parse id.");
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
                    Image = result.Image
                };
                return Ok(responseResult);
            }
            throw new NotFoundException($"Item with id {id} was not found.");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateItem([FromBody] ItemModel item)
        {
            // Parse category Id and convert it to guid

            if (!Guid.TryParse(item.CategoryId, out var categoryIdGuid))
            {
                throw new BadRequestException("Unable to read the category.");
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
                CategoryId = categoryIdGuid,
                OwnerId = _userId,
                Image = item.Image,
                Note = item.Note, 
                IsDeleted = false
            };

            // Add item to the batabase and add it to the category
            await _context.Items.AddAsync(newItem);

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
                        Image = newItem.Image
                    },
                    category = new CategoryDTO
                    {
                        _id = updatedCategory.Id.ToString(),
                        Category = updatedCategory.CategoryName,
                        Owner = updatedCategory.OwnerId.ToString(),
                        Items = updatedCategory.Items.Where(i => i.IsDeleted == false).Select(i => i.Id.ToString()).ToList()
                    }

                };
                return Ok(responseResult);
            }
            throw new BadRequestException("Failed to create a new item.");
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateItem(string id, [FromBody] ItemModel item)
        {
            // Parse Ids
            if (!Guid.TryParse(id, out var guidId) || !Guid.TryParse(item.CategoryId, out var guidCategoryId))
            {
                throw new BadRequestException("Not valid the item's Id or the category's Id.");
            }
            //Find the updated item
            var updatedItem = await _context.Items.Include(i => i.Category).FirstOrDefaultAsync(item => item.Id == guidId);

            if (updatedItem == null)
            {
                throw new NotFoundException($"The item with id {id} was not found.");
            }
            //Find the current category and a new category

            Category oldCategory = updatedItem.Category;
            Category newCategory;

            if (guidCategoryId != updatedItem.CategoryId)
            {
                newCategory = await _context.Categories.Include(c => c.Items).FirstOrDefaultAsync(cat => cat.Id == guidCategoryId);

                if (newCategory == null)
                {
                    throw new NotFoundException($"The category with id {updatedItem.CategoryId}, {guidCategoryId} was not found.");
                }
            } else
            {
                newCategory = oldCategory;
            }

            //Update item
            updatedItem.ItemName = item.Name;
            updatedItem.CategoryId = guidCategoryId;
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
                        Image = updatedItem.Image
                    },
                    deleteFromCategory = new CategoryDTO
                    {
                        _id = oldCategory.Id.ToString().ToLower(),
                        Category = oldCategory.CategoryName,
                        Owner = oldCategory.OwnerId.ToString().ToLower(),
                        Items = oldCategory.Items.Where(i => i.IsDeleted == false).Select(i => i.Id.ToString().ToLower()).ToList()
                    },
                    addToCategory = new CategoryDTO
                    {
                        _id = newCategory.Id.ToString().ToLower(),
                        Category = newCategory.CategoryName,
                        Owner = newCategory.OwnerId.ToString().ToLower(),
                        Items = newCategory.Items.Where(i => i.IsDeleted == false).Select(i => i.Id.ToString().ToLower()).ToList()
                    }
                };
                return Ok(responseResult);
            }
            throw new BadRequestException("Failed to update the item.");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteItem(string id)
        {
            if (!Guid.TryParse(id, out var guidId))
            {
                throw new BadRequestException("Cannot parse the item's id.");
            }
            var deletedItem = await _context.Items.Include(i => i.Category).FirstOrDefaultAsync(item => item.Id == guidId);
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
                        Note = deletedItem.Note
                    },
                    updatedCategory = new CategoryDTO
                    {
                        _id = uptadedCategory.Id.ToString(),
                        Owner = uptadedCategory.OwnerId.ToString(),
                        Category = uptadedCategory.CategoryName,
                        Items = uptadedCategory.Items.Where(i=> i.IsDeleted == false).Select(i => i.Id.ToString()).ToList()
                    }
                };
                return Ok(responseResult);
            }
            throw new BadRequestException($"Failed to delete the item with {id} id.");
        }
    }
}
