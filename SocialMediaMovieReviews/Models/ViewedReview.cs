namespace SocialMediaMovieReviews.Models
{
    public class ViewedReview
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public Review Review { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
