using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers;

namespace shoppingify_backend.Models
{
    public class ApplicationContext: DbContext
    {
        private readonly string _currentUserId;
        public ApplicationContext(DbContextOptions<ApplicationContext> options, UserResolverService userResolverService): base(options)
        {
            _currentUserId = userResolverService.GetCurrentUserId();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add query filters to the context to show only entities related to the current user
            modelBuilder.Entity<Item>()
                .HasQueryFilter(i => i.OwnerId == _currentUserId);
            modelBuilder.Entity<Category>()
                .HasQueryFilter(i => i.OwnerId == _currentUserId);
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }


    }
}

