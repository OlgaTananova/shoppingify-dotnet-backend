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

        [HttpPatch("/ShoppingLists/updqty")]
        [Authorize]
        public async Task<IActionResult> ChangeItemQuantity([FromBody] ChangeItemQuantityModel changeItemQuantity)
        {
            var result = await _shoppingListItemService.ChangeItemQuantity(changeItemQuantity);
            return Ok(result);
        }

        [HttpPatch("/ShoppingLists/updstatus")]
        [Authorize]
        public async Task<IActionResult> ChangeItemStatus([FromBody] ChangeItemStatusModel changeItemStatus)
        {
            var result = await _shoppingListItemService.ChangeItemStatus(changeItemStatus);
            return Ok(result);
        }

        [HttpPatch("/ShoppingLists/updItemUnits")]
        [Authorize]
        public async Task<IActionResult> ChangeItemUnits([FromBody] ChangeItemUnitsModel changeItemUnits)
        {
            var result = await _shoppingListItemService.ChangeItemUnits(changeItemUnits);
            return Ok(result);
        }

        [HttpPatch("/ShoppingLists/updItemPricePerUnit")]
        [Authorize]
        public async Task<IActionResult> ChangeItemPricePerUnit([FromBody] ChangeItemPricePerUnitModel changeItemPricePerUnit)
        {
            var result = await _shoppingListItemService.ChangeItemPricePerUnit(changeItemPricePerUnit);
            return Ok(result);
        }

        [HttpPatch("/ShoppingLists/updItemPrice")]
        [Authorize]
        public async Task<IActionResult> ChangeItemPrice([FromBody] ChangeItemPriceModel changeItemPrice)
        {
            var result = await _shoppingListItemService.ChangeItemPrice(changeItemPrice);
            return Ok(result);
        }

    }
}
