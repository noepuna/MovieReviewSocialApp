using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialMediaMovieReviews.Models;
using SocialMediaMovieReviews.Models;

namespace SocialMediaMovieReviews.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<SocialMediaMovieReviews.Models.Movie>? Movie { get; set; }
        public DbSet<SocialMediaMovieReviews.Models.Review>? Review { get; set; }
        public DbSet<SocialMediaMovieReviews.Models.ReviewLike>? ReviewLikes { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReviewLike>()
                .HasOne(rl => rl.Review)
                .WithMany(r => r.Likes)
                .HasForeignKey(rl => rl.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull); // Specify the desired cascade behavior

            modelBuilder.Entity<ReviewLike>()
                .HasOne(rl => rl.User)
                .WithMany(r => r.Likes)
                .HasForeignKey(rl => rl.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull); // Specify the desired cascade behavior

            // Other entity configurations...

            base.OnModelCreating(modelBuilder);
        }


    }
}