using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EventFinderAPI.Models
{
    public class AppUser:IdentityUser
    {
        public string? pfpUrl = null;
        public ICollection<Item>ItemsAdded { get; }

        public ICollection<Review> ReviewsAdded { get; }
    }
}
