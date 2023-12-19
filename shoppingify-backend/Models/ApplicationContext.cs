using Microsoft.EntityFrameworkCore;

namespace shoppingify_backend.Models
{
    public class ApplicationContext: DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }


    }
}

