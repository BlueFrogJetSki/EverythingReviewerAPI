using reviews4everything.Models;

namespace reviews4everything.DTOs
{
    public class ProfileDTO
    {
        public string? PfpUrl { get; set; }
        public string? Username { get; set; }
        public ICollection<ReviewDTO>? Reviews { get; set; }

        public ProfileDTO()
        {

        }

        public ProfileDTO(AppUser user)
        {
            PfpUrl = user.PfpUrl;
            Username = user.UserName;
            Reviews = user.ReviewsAdded?
                .Select(r => new ReviewDTO(r))
                .ToList();

            Console.WriteLine(user.ToString());
        }

    }
}
