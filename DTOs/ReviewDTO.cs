using reviews4everything.Models;

namespace reviews4everything.DTOs
{
    public class ReviewDTO
    {
        public ReviewDTO(Review review)
        {
            Id = review.Rid.ToString();
            Text = review.Text;
            Rating = review.Rating;
            Username = review.CreatedBy.UserName;
            CreatedAt = review.createdAt;
            PfpUrl = review.CreatedBy.PfpUrl;
            Item = review.Item.Name;

        }
        public string Id { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }

        public string Username { get; set; }

        public string PfpUrl { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public string Item { get; set; }
    }
}
