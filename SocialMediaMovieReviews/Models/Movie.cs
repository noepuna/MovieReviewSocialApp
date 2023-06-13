using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SocialMediaMovieReviews.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Director { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string PosterURL { get; set; }
        public string TrailerURL { get; set; }
        public ICollection<Review> Reviews { get; set; }

        public float getAverageRating()
        {
            var totalAverage = 0.0;
            foreach (var review in Reviews)
            {
                totalAverage += review.Rating;
            }
            return (float)(totalAverage / Reviews.Count);
        }
    }
}
