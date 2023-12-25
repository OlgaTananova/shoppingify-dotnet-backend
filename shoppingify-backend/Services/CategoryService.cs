using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;
using System.ComponentModel;

namespace shoppingify_backend.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetCategories();
        Task<CategoryDTO> CreateCategory(CategoryModel category);
    }
    public class CategoryService : ICategoryService { 

        private readonly ApplicationContext _context;
        private readonly IUserResolverService _userResolverService;

        public CategoryService(ApplicationContext context, IUserResolverService userResolverService)
        {
            _context = context;
            _userResolverService = userResolverService;
        }

        public async Task<List<CategoryDTO>> GetCategories() {

            var result = await _context.Categories
                                           .Include(c => c.Items)
                                           .Select(cat => new CategoryDTO
                                           {
                                               _id = cat.Id.ToString().ToLower(),
                                               Category = cat.CategoryName,
                                               Owner = cat.OwnerId.ToString().ToLower(),
                                               Items = cat.Items.Select(i => i.Id.ToString().ToLower()).ToList()
                                           })
                                           .ToListAsync();
            if (result.Any())
            {
                return result;
            }
            return new List<CategoryDTO>();

        }

        public async Task<CategoryDTO> CreateCategory(CategoryModel category)
        {
            string userId = _userResolverService.GetCurrentUserId();

            if (!Guid.TryParse(userId, out var userIdGuid))
            {
                throw new BadRequestException("Failed to read the userId.");
            }

            var newCategory = new Category
            {
                CategoryName = category.Category,
                OwnerId = userIdGuid
            };

            _context.Categories.Add(newCategory);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return new CategoryDTO
                {
                    _id = newCategory.Id.ToString(),
                    Category = newCategory.CategoryName,
                    Owner = newCategory.OwnerId.ToString(),
                    Items = new List<string>()
                };
            }

            throw new BadRequestException("Failed to create the category.");

        }
    }
}
