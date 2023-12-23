using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;
using shoppingify_backend.Services;
using System.Data;
using System.Security.Claims;

namespace shoppingify_backend.Controllers
{
    [Route("[controller]")]
    [ApiController] // Allows automatically validate requests' body parameters
    public class CategoriesController : ControllerBase
    {
        //private readonly ApplicationContext _context;
        //private readonly string _userId;
        private readonly ICategoryService _categoryService;
        public CategoriesController(ApplicationContext context, IUserResolverService userResolverService, ICategoryService categoryService)
        {
            //_context = context;
            _categoryService = categoryService;
            //_userId = userResolverService.GetCurrentUserId();

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _categoryService.GetCategories();
            return Ok(result);

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryModel category)
        {
            var result = await _categoryService.CreateCategory(category);
            return Ok(result);
        }
    }
}
