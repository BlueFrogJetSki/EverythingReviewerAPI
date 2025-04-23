using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventFinderAPI.Models
{
    public class Item
    {
        [Key]
        [Required]
        public int Wid { get; set; }

        //Word lenght one, relates to other words
        [Required(ErrorMessage = "Title is required")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 250 characters")]
        public string Name { get; set; }

        public string NormalizedName { get; set; } 

        [Required]
        [ForeignKey("CreatedBy")]
        public string Uid { get; set; }

        public AppUser CreatedBy { get; set; }


        public DateTimeOffset createdAt { get; set; } = DateTimeOffset.Now.ToUniversalTime();

        public float avgRating { get; set; }

        public long RatingSum { get; set; } = 0;

        public long RatingCount { get; set; } = 0;

        public ICollection<Review> Reviews { get; set; }







    }
}
