using reviews4everything.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace reviews4everything.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }

        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Item>()
               .HasIndex(u => u.NormalizedName)
               .IsUnique();// 👈 enforce uniqueness

            modelBuilder.Entity<Item>()
               .Property(u => u.createdAt).HasColumnType("timestamptz");

            modelBuilder.Entity<AppUser>()
               .HasIndex(u => u.Email)
               .IsUnique(); // 👈 enforce uniqueness

            modelBuilder.Entity<AppUser>()
              .HasIndex(u => u.NormalizedUserName)
              .IsUnique(); // 👈 enforce uniqueness


            modelBuilder.Entity<Item>()
                .HasOne(p => p.CreatedBy) // Assuming a navigation property exists in Post class
                .WithMany(a => a.ItemsAdded) // Assuming AppUser has a collection of Posts
                .HasForeignKey(p => p.Uid); // Foreign key property in Post

            modelBuilder.Entity<Review>()
               .Property(u => u.createdAt).HasColumnType("timestamptz");

            modelBuilder.Entity<Review>()
               .Property(u => u.updatedAt).HasColumnType("timestamptz");

            modelBuilder.Entity<Review>()
                .HasOne(p => p.CreatedBy) // Assuming a navigation property exists in Post class
                .WithMany(a => a.ReviewsAdded) // Assuming AppUser has a collection of Posts
                .HasForeignKey(p => p.Uid); // Foreign key property in Post

            modelBuilder.Entity<Review>()
                .HasOne(p => p.Item) // Assuming a navigation property exists in Post class
                .WithMany(a => a.Reviews) // Assuming AppUser has a collection of Posts
                .HasForeignKey(p => p.Wid); // Foreign key property in Post
        }
    }
}
