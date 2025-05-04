using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reviews4everything.Models;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using reviews4everything.DTOs;
using reviews4everything.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Caching.Memory;


namespace reviews4everything.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IMemoryCache _cache;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(AppDbContext context, IMemoryCache cache, ILogger<ReviewsController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }


        // GET: api/Reviews/5
        //get reviews for an item with itemName
        [HttpGet("{itemName}")]
        public async Task<ActionResult<ICollection<ReviewDTO>>> GetReview(string itemName)
        {
            var normalisedItemName = itemName.ToUpperInvariant();

            var item = await _context.Items
               .Where(i => i.NormalizedName == normalisedItemName)
               .Select(i => new { i.Wid })
               .FirstOrDefaultAsync();

            if (item == null) { return NotFound(); }

            var reviews = await _context.Reviews.Include(r => r.CreatedBy).Include(r => r.Item).Where(r => (r.Wid == item.Wid)).Take(50).ToListAsync();

            if (reviews == null)
            {
                return NotFound();
            }

            var reviewDTOs = reviews.Select(r => new ReviewDTO(r)).ToList();

            return reviewDTOs;
        }

        // GET: recent
        [HttpGet("Recent")]
        public async Task<ActionResult<ICollection<ReviewDTO>>> GetRecentReviews([FromQuery] int num)
        {

            var reviews = await _context.Reviews.Include(r => r.CreatedBy).Include(r => r.Item).OrderByDescending(e => e.createdAt).Take(num).ToListAsync();

            return reviews.Select(x => new ReviewDTO(x)).ToList();
        }

        // GET: api/reviews?page=page&limit=limit
        [HttpGet()]
        public async Task<ActionResult<ICollection<ReviewDTO>>> GetPaginatedReviews([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            string cacheKey = $"reviews page {page}";
            var startTime = DateTime.Now;

            // Try to get from cache
            if (_cache.TryGetValue(cacheKey, out ICollection<ReviewDTO>? cachedReviews))
            {
                var cacheStopTime = DateTime.Now;
                _logger.LogInformation("Took {time} to query page {page} at limit {} from cache", (cacheStopTime - startTime).TotalMilliseconds, page, limit);
                return Ok(cachedReviews); // return cached
            }

            //cache miss
            var reviews = await _context.Reviews
                .Include(r => r.CreatedBy)
                .Include(r => r.Item)
                .OrderByDescending(e => e.createdAt)
                .Skip((page - 1) * limit)
                .Take(limit).Select(x => new ReviewDTO(x))
                .ToListAsync();
            var stopTime = DateTime.Now;

            // Set cache with expiration
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(45)).SetAbsoluteExpiration(TimeSpan.FromSeconds(3600));

            _cache.Set(cacheKey, reviews, cacheOptions);

            _logger.LogInformation("Took {time} to query page {page} at limit {}", (stopTime - startTime).TotalMilliseconds, page, limit);
            return reviews;
        }

        [HttpGet("count")]
        public ActionResult<int> GetTotalCount()
        {
            var total_review_count = _context.Reviews.Count();

            return total_review_count;
        }




        // PUT: api/Reviews/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(int id, Review review)
        {
            if (id != review.Rid)
            {
                return BadRequest();
            }

            if (!ReviewBelongToUser(review))
            {
                return Unauthorized();
            }

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Reviews
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // if the target item for the review doesnt exist, create the item first

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public async Task<ActionResult<Review>> PostReview([FromQuery] String ItemName, Review review)
        {

            if (review == null)
            {
                return BadRequest(new { message = "review cannot be null" });
            }

            var userId = User.FindFirst("id")?.Value;
            Console.WriteLine($"userid: {userId}");
            if (userId == null || (await _context.Users.FindAsync(userId) == null))
            {

                return Unauthorized();
            }

            var normalizedItemName = ItemName.ToUpperInvariant();

            Item? item = await _context.Items.FirstOrDefaultAsync(i => i.NormalizedName.Equals(normalizedItemName));
            //create new item if item == null
            if (item == null)
            {
                var sb = new StringBuilder();
                sb.Append(normalizedItemName[0]);
                sb.Append(normalizedItemName.Substring(1).ToLowerInvariant());

                var itemName = sb.ToString();

                //create item
                item = new Item()
                {
                    Name = itemName,
                    NormalizedName = normalizedItemName,
                    Uid = userId,
                };

                _context.Items.Add(item);
                await _context.SaveChangesAsync();
            }

            //update values related to calculating avgRating
            RecalculateAverageRating(item, review.Rating);


            review.Uid = userId;
            review.Wid = item.Wid;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();



            return Ok(new { message = "Review created successfully" });
        }

        // DELETE: api/Reviews/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            if (!ReviewBelongToUser(review))
            {
                return Unauthorized();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Rid == id);
        }

        private bool ReviewBelongToUser(Review review)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) { return false; }
            return (userId.Equals(review.Uid));
        }

        private void RecalculateAverageRating(Item item, int rating)
        {
            item.RatingCount++;
            item.RatingSum += rating;
            item.avgRating = item.RatingSum / item.RatingCount;
        }
    }
}
