namespace SocialMediaMovieReviews.Models
{
    public class ReviewLike
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int ReviewId { get; set; }
        public Review Review { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public bool isLiked { get; set; }
    }
}
