using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shoppingify_backend.Models.Entities;
using shoppingify_backend.Services;

namespace shoppingify_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShoppingListsController : ControllerBase
    {
        private readonly IShoppingListService _shoppingListService;

        public ShoppingListsController(IShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        [HttpPost]
        [Authorize]

        public async Task<IActionResult> CreateShoppingList([FromBody] ShoppingListModel shoppingListData)
        {
            var result = await _shoppingListService.CreateShoppingList(shoppingListData);

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetShoppingLists()
        {
            var result = await _shoppingListService.GetShoppingLists();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteShoppingList(string id)
        {
            var result = await _shoppingListService.DeleteShoppingList(id);
            return Ok(result);
        }
    }
}
