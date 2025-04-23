using EventFinderAPI.Models;

namespace EventFinderAPI.DTOs
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
            PfpUrl = user.pfpUrl;
            Username = user.UserName;
            Reviews = user.ReviewsAdded?
                .Select(r => new ReviewDTO(r))
                .ToList();

            Console.WriteLine(user.ToString());
        }

    }
}
