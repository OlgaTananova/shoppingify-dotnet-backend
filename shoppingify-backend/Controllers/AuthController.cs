using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using shoppingify_backend.Data;
using shoppingify_backend.Helpers;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;
using shoppingify_backend.Services;
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
        private readonly string _userId;
        private readonly IUserResolverService _userResolverService;
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AuthContext context, IConfiguration configuration, IUserResolverService userResolverService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
            _userResolverService = userResolverService;
            _userId = userResolverService.GetCurrentUserId();
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel registerModel)
        {
            var userexist = await _userManager.FindByEmailAsync(registerModel.Email);
            if (userexist != null)
            {
                throw new ConflictException("The user with such email already exists.");
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

            return Ok(new
            {
                name = newuser.UserName,
                email = newuser.Email
            });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
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

                    return Ok(new { message = "Token was sent in cookies." });
                }

            }
            throw new ForbiddenException("Incorrect password or email.");
        }

        [HttpGet("users/me")]
        [Authorize]

        public async Task<IActionResult> GetCurrentUser()
        {

            var user = await _userManager.FindByIdAsync(_userId);
            if (user != null)
            {
                return Ok(new
                {
                    name = user.UserName,
                    email = user.Email
                });
            }

            throw new NotFoundException("The user was not found.");

        }

        [HttpPatch("users/me")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel userData)
        {
            var user = await _userManager.FindByIdAsync(_userId);
            if (user == null)
            {
                throw new NotFoundException($"Failed to find the user with {_userId} id.");
            }

            if (user.Email != userData.Email)
            {
                var userWithExistingEmail = _userManager.FindByEmailAsync(userData.Email);

                if (userWithExistingEmail != null)
                {
                    throw new ConflictException($"User with {userData.Email} email already exists.");
                }
            }

            user.Email = userData.Email;
            user.UserName = userData.Name;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new
                {
                    message = "User's profile was successfully updated.",
                    name = user.UserName,
                    email = user.Email
                });
            }
            throw new BadRequestException("Failed to update the user's profile.");
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

            return Ok(new { message = "You were successfully logged out." });
        }

    }
}
