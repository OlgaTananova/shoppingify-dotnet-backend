using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;
using System.Data;
using System.Security.Claims;

namespace shoppingify_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        public CategoriesController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _context.Categories.ToArrayAsync();

            if(result.Length > 0)
            {
                return Ok(result);
            }
            throw new NotFoundException("There are no categories.");

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromBody]CategoryModel category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                throw new ForbiddenException("Unauthorized action.");
            }
            var newCategory = new Category
            {
                CategoryName = category.CategoryName,
                OwnerId = userId,
            };


            _context.Categories.Add(newCategory);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Ok(newCategory);
            }

            throw new BadRequestException("Failed to create the category.");

        }
    }
}
