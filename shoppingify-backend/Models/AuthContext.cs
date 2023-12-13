using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

