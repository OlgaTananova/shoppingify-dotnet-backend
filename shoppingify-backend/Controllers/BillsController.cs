using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using shoppingify_backend.Models.ValidationModels;
using shoppingify_backend.Services;

namespace shoppingify_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        private readonly IBillService _billService;

        public BillsController(IBillService billService)
        {
            _billService = billService;
        }

        [HttpPost("/upload-bill")]
        [Authorize]

        public async Task<IActionResult> UploadBill()
        {
            var result = await _billService.UploadBill();
            return Ok(result);
        }

        [HttpPost("/upload-list")]
        [Authorize]
        public async Task<IActionResult> UploadList(UploadShoppingListModel uploadShoppingList)
        {
            var result = await _billService.UploadShoppingList(uploadShoppingList);
            return Ok(result);
        }
    }
}
