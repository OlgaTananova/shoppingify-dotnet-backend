using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Models.Entities;

namespace shoppingify_backend.Models
{
    // DbContext to connect to the database with users
    public class AuthContext : IdentityDbContext<ApplicationUser>
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
        }
    
    }
}

