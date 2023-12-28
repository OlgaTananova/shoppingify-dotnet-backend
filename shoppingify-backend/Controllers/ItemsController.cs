using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models.ValidationModels;
using shoppingify_backend.Services;

namespace shoppingify_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        [Authorize]

        public async Task<IActionResult> GetAllItems()
        {
            var result = await _itemService.GetAllItems();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetItem(string id)
        {
            var result = await _itemService.GetItem(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateItem([FromBody] ItemModel item)
        {
            var result = await _itemService.CreateItem(item);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateItem(string id, [FromBody] ItemModel item)
        {
            var result = await _itemService.UpdateItem(id, item);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteItem(string id)
        {
            var result = await _itemService.DeleteItem(id);
            return Ok(result);
        }
    }
}
