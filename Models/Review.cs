using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventFinderAPI.Models
{
    public class Review
    {
        [Key]
        [Required]
        public int Rid { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 5, ErrorMessage = "Text must be between 5 and 300 characters")]
        public string Text { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Please enter valid rating 1-5")]
        public int Rating { get; set; }

        [ValidateNever]
        [ForeignKey("CreatedBy")]
        public string Uid { get; set; }

        [ValidateNever]
        public AppUser CreatedBy { get; set; }

        [ForeignKey("Item")]
        public int Wid { get; set; }

        [ValidateNever]
        public Item Item { get; set; }

        public DateTimeOffset createdAt { get; set; } = DateTimeOffset.Now.ToUniversalTime();

        public DateTimeOffset updatedAt { get; set; } = DateTimeOffset.Now.ToUniversalTime();

    }
}
