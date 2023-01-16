using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using System.ComponentModel.DataAnnotations.Schema;



namespace OnlineShop.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Photo> Photos { get; set; } 
        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // definire primary key compus
            modelBuilder.Entity<Cart>()
                .HasKey(ab => new { ab.ArticleId, ab.UserId });


            // definire relatii cu modelele Bookmark si Article (FK)
            modelBuilder.Entity<Cart>()
                .HasOne(ab => ab.Article)
                .WithMany(ab => ab.Carts)
                .HasForeignKey(ab => ab.ArticleId);

            modelBuilder.Entity<Cart>()
                .HasOne(ab => ab.User)
                .WithMany(ab => ab.Carts)
                .HasForeignKey(ab => ab.UserId);
        }
    }
}