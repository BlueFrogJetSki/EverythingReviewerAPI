using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace reviews4everything.Models
{
    public class AppUser:IdentityUser
    {

        public string? PfpUrl { get; set; }
        public ICollection<Item>ItemsAdded { get; }

        public ICollection<Review> ReviewsAdded { get; }
    }
}
