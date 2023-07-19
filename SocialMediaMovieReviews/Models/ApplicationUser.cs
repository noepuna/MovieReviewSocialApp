using Microsoft.AspNetCore.Identity;
using SocialMediaMovieReviews.Models;
using System.Collections;

namespace SocialMediaMovieReviews.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string User_Name { get; set; }

        public ICollection<Review> Reviews { get; set; }
        public ICollection<ReviewLike> Likes { get; set; }
        public ICollection<ViewedReview> Views { get; set; }
        public ICollection<ApplicationUser> Followers { get; set; }
        public ICollection<ApplicationUser> Following { get; set; }

        // the average percent of likes for each review
        public double GetLikePercentage()
        {
            var totalreviews = Reviews.Count();
            var totalpercentage = 0;
            foreach (var review in Reviews)
            {
                var reviewtotallikes = review.Likes.Count();
                var reviewlikescount = review.Likes.Where(l => l.isLiked == true).Count();

                var reviewlikepercentage = reviewlikescount / reviewtotallikes;
                totalpercentage += reviewlikepercentage;
                
                
            }
            double likepercentage = (totalpercentage / totalreviews)*100;
            return 59.8;
        }
    }
}
