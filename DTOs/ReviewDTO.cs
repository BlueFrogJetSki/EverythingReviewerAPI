using EventFinderAPI.Models;

namespace EventFinderAPI.DTOs
{
    public class ReviewDTO
    {
        public ReviewDTO(Review review)
        {
            Text = review.Text;
            Rating = review.Rating;
            Username = review.CreatedBy.UserName;
            CreatedAt = review.createdAt;
            PfpUrl = review.CreatedBy.pfpUrl;
        }
        public string Text { get; set; }
        public int Rating { get; set; }

        public string Username { get; set; }

        public string PfpUrl { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
