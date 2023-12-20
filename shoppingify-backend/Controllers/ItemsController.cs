using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;

namespace shoppingify_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly string _userId;

        public ItemsController(ApplicationContext context, UserResolverService userResolverService)
        {
            _context = context;
            _userId = userResolverService.GetCurrentUserId();
        }

        [HttpGet]
        [Authorize]

        public async Task<IActionResult> GetAllItems()
        {
            var result = await _context.Items.ToListAsync();

            if (result.Count > 0)
            {
                var responseResult = result.Select(i => new
                {
                    _id = i.Id,
                    categoryId = i.CategoryId,
                    name = i.ItemName,
                    owner = i.OwnerId,
                    note = i.Note,
                    image = i.Image
                }).ToList();
                return Ok(responseResult);
            }
            return Ok(new List<object>());
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
                Note = item.Note
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
                    item = new
                    {
                        _id = newItem.Id,
                        name = newItem.ItemName,
                        categoryId = newItem.CategoryId,
                        owner = newItem.OwnerId,
                        note = newItem.Note,
                        image = newItem.Image
                    },
                    category = new
                    {
                        _id = updatedCategory.Id,
                        category = updatedCategory.CategoryName,
                        owner = updatedCategory.OwnerId,
                        items = updatedCategory.Items.Select(i => i.Id).ToList()
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
            var updatedItem = await _context.Items.FindAsync(guidId);
           
            if (updatedItem == null)
            {
                throw new NotFoundException($"The item with id {id} was not found.");
            }
        //Find the current category and a new category
            var oldCategory = await _context.Categories.Include(c => c.Items).FirstOrDefaultAsync(cat => cat.Id == updatedItem.CategoryId);

            var newCategory = await _context.Categories.Include(c => c.Items).FirstOrDefaultAsync(cat => cat.Id == guidCategoryId);

            if (oldCategory == null || newCategory == null)
            {
                throw new NotFoundException($"The category with id {updatedItem.CategoryId}, {guidCategoryId} was not found.");
            }
        //Update item
            updatedItem.ItemName = item.Name;
            updatedItem.CategoryId = guidCategoryId;
            updatedItem.Note = item.Note;
            updatedItem.Image = item.Image;

            _context.Items.Update(updatedItem);
            var result = await _context.SaveChangesAsync();
            if(result > 0)
            {
                var responseResult = new
                {
                    updatedItem = new
                    {
                        _id = updatedItem.Id,
                        name = updatedItem.ItemName,
                        categoryid = updatedItem.CategoryId,
                        note = updatedItem.Note,
                        image = updatedItem.Image
                    },
                    deleteFromCategory = new
                    {
                        _id = oldCategory.Id,
                        category = oldCategory.CategoryName,
                        owner = oldCategory.OwnerId,
                        items = oldCategory.Items
                    },
                    addToCategory = new
                    {
                        _id = newCategory.Id,
                        category = newCategory.CategoryName,
                        owner = newCategory.OwnerId,
                        items = newCategory.Items
                    }
                };
                return Ok(responseResult);
            }
            throw new BadRequestException("Failed to update the item.");
        }
    }
}
