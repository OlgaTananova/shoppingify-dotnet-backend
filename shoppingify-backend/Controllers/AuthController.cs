using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using shoppingify_backend.Data;
using shoppingify_backend.Models;

namespace shoppingify_backend.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AuthContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AuthContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please, provide all the reqired fields.");
            }
            try
            {
                var userExist = await _userManager.FindByEmailAsync(registerModel.Email);
                if (userExist != null)
                {
                    return BadRequest($"User {registerModel.Email} already exists.");
                }

                ApplicationUser newUser = new ApplicationUser()
                {
                    UserName = registerModel.Name,
                    Email = registerModel.Email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var result = await _userManager.CreateAsync(newUser, registerModel.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("errors", error.Description); // Add errors to the ModelState
                    }
                    return BadRequest(ModelState);
                }

                return Ok("User was created.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while processing your request", Detail = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var exitstingUser = await _userManager.FindByEmailAsync(loginModel.Email);
                if (exitstingUser != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(exitstingUser, loginModel.Password, isPersistent: true, false);
                    if (result.Succeeded)
                    {
                        // Token generation
                        //var token = await TokenGenerator.GenerateJwtToken(exitstingUser, _configuration);
                        //var cookieOptions = new CookieOptions
                        //{
                        //    HttpOnly = true,
                        //    Expires = DateTime.UtcNow.AddDays(8), // Set the same expiry as your token
                        //  // Secure = true, // Uncomment this if you're using HTTPS
                        //    SameSite = SameSiteMode.None // Helps mitigate CSRF attacks
                        //};
                        //// Attach cookies to the response
                        //Response.Cookies.Append("Token", token, cookieOptions);
                        return Ok("Token was sent in cookies");
                    }

               }
                return Unauthorized("Invalid email or password.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while processing your request", Detail = ex.Message });

            }

        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            //foreach (var cookie in Request.Cookies.Keys)
            //{
            //    Response.Cookies.Delete(cookie);
            //}
            await _signInManager.SignOutAsync();

            return Ok("You were successfully logged out.");
        }

    }
}
