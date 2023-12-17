using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using shoppingify_backend.Data;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;

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
        private readonly ILogger<AuthController> _logger;
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AuthContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)

            {
                return BadRequest(ModelState);
            }
            var userexist = await _userManager.FindByEmailAsync(registerModel.Email);
            if (userexist != null)
            {
                throw new ConflictException("the user with such email already exists.");
            }

            ApplicationUser newuser = new ApplicationUser()
            {
                UserName = registerModel.Name,
                Email = registerModel.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(newuser, registerModel.Password);
            if (!result.Succeeded)
            {
                var errors = new StringBuilder();
                foreach (var er in result.Errors)
                {
                    errors.Append(er.Description);
                    errors.Append(" ");
                }
                throw new BadRequestException(errors.ToString());
            }

            return Ok("User was created.");

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exitstingUser = await _userManager.FindByEmailAsync(loginModel.Email);
            if (exitstingUser != null)
            {
                var result = await _signInManager.PasswordSignInAsync(exitstingUser, loginModel.Password, isPersistent: true, false);
                if (result.Succeeded)
                {
                    // Custom token generation
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
            throw new ForbiddenException("Incorrect password or email.");

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
