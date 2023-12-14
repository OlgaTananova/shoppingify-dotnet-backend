using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace shoppingify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetHome()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets user ID
            var userName = User.Identity.IsAuthenticated;
            Console.WriteLine(userId + userName);
            return Ok("Welcome!");
        }
    }
}
