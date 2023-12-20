using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;
using System.Data;
using System.Security.Claims;

namespace shoppingify_backend.Controllers
{
    [Route("[controller]")]
    [ApiController] // Allows automatically validate requests' body parameters
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly string _userId;
        public CategoriesController(ApplicationContext context, UserResolverService userResolverService)
        {
            _context = context;
            _userId = userResolverService.GetCurrentUserId();

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _context.Categories
                                            .Include(c => c.Items)
                                            .Select(cat => new
                                            {
                                                _id = cat.Id,
                                                category = cat.CategoryName,
                                                owner = cat.OwnerId,
                                                items = cat.Items.Select(i => i.Id).ToList()
                                            })
                                            .ToListAsync();
            if (result.Any())
            {
               return Ok(result);
            }
            return Ok(new List<object>());
            
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromBody]CategoryModel category)
        {
            var newCategory = new Category
            {
                CategoryName = category.Category,
                OwnerId = _userId
            };

            _context.Categories.Add(newCategory);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Ok(new
                {
                    _id = newCategory.Id,
                    category = newCategory.CategoryName,
                    owner = newCategory.OwnerId
                });
            }

            throw new BadRequestException("Failed to create the category.");

        }
    }
}
