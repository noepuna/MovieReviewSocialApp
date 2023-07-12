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
    }
}
