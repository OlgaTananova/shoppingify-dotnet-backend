﻿using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers;
using shoppingify_backend.Models.Entities;
using shoppingify_backend.Services;

namespace shoppingify_backend.Models
{
    public class ApplicationContext : DbContext
    {
        private readonly string _currentUserId;
        private readonly IUserResolverService _userResolverService;
        public ApplicationContext(DbContextOptions<ApplicationContext> options, IUserResolverService userResolverService) : base(options)
        {
            _userResolverService = userResolverService;
            _currentUserId = userResolverService.GetCurrentUserId();

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add query filters to the context to show only entities related to the current user
            modelBuilder.Entity<Item>()
                .HasQueryFilter(i => i.OwnerId.ToString() == _currentUserId);
            modelBuilder.Entity<Category>()
                .HasQueryFilter(i => i.OwnerId.ToString() == _currentUserId);
            modelBuilder.Entity<ShoppingList>()
                .HasQueryFilter(sl => sl.OwnerId.ToString() == _currentUserId);
            modelBuilder.Entity<ShoppingListItem>()
                .HasQueryFilter(sli => sli.OwnerId.ToString() == _currentUserId);


            //Relations for Item and Category
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Items)
                .WithOne(i => i.Category)
                .HasForeignKey(i => i.CategoryId);

            modelBuilder.Entity<Item>()
                .HasMany(i => i.ShoppingListItems)
                .WithOne(i => i.Item)
                .HasForeignKey(i => i.ItemId);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryId);

            // Relations for ShoppingList
            modelBuilder.Entity<ShoppingList>()
                .HasMany(sl => sl.ShoppingListItems)
                .WithOne(sli => sli.ShoppingList)
                .HasForeignKey(sl => sl.ShoppingListId);

            // Relations for ShoppingListItem

            modelBuilder.Entity<ShoppingListItem>()
                .HasOne(sli => sli.ShoppingList)
                .WithMany(s => s.ShoppingListItems)
                .HasForeignKey(sli => sli.ShoppingListId);

            modelBuilder.Entity<ShoppingListItem>()
                .HasOne(sli => sli.Item)
                .WithMany(i => i.ShoppingListItems)
                .HasForeignKey(sli => sli.ItemId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ShoppingListItem>()
                .HasOne(sli => sli.Category)
                .WithMany(c => c.ShoppingListItems) 
                .HasForeignKey(sli => sli.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }

        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<ShoppingListItem> ShoppingListItems { get; set; }


    }
}

