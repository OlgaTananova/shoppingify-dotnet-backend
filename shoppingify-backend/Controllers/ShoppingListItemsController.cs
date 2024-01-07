using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using shoppingify_backend.Models.ValidationModels;
using shoppingify_backend.Services;

namespace shoppingify_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShoppingListItemsController : ControllerBase
    {
        
        private readonly IShoppingListItemService _shoppingListItemService;

        public ShoppingListItemsController(IShoppingListItemService shoppingListItemService)
        {
            _shoppingListItemService = shoppingListItemService;
        }

        [HttpPut("/ShoppingLists")]
        [Authorize]
        public async Task<IActionResult> AddItemToShoppingList([FromBody] AddItemToShoppingListModel addItemToSL)
        {
            var result = await _shoppingListItemService.AddItemToShoppingList(addItemToSL);
            return Ok(result);
        }
    }
}
