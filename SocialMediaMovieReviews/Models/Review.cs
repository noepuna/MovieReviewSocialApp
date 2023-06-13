using SocialMediaMovieReviews.Models;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaMovieReviews.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Text { get; set; }
        [Range(0, 10)]
        public float Rating { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<ReviewLike> Likes { get; set; }

    }
}
