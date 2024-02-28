using Microsoft.IdentityModel.Tokens;
using shoppingify_backend.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace shoppingify_backend.Data
{
    //public static class TokenGenerator
    //{

    //    public async static Task<string> GenerateJwtToken(ApplicationUser user, IConfiguration configuration)
    //    {
    //        var claims = new List<Claim>()
    //        {
    //            new Claim(ClaimTypes.Name, user.UserName),
    //            new Claim(ClaimTypes.NameIdentifier, user.Id),
    //            new Claim(JwtRegisteredClaimNames.Email, user.Email),
    //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //        };

    //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
    //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //        var token = new JwtSecurityToken(
    //            issuer: configuration["JWT:Issuer"],
    //            audience: configuration["JWT:Audience"],
    //            claims: claims,
    //            expires: DateTime.Now.AddDays(8),
    //            signingCredentials: creds);

    //        return new JwtSecurityTokenHandler().WriteToken(token);
    //    }

    //}
}
