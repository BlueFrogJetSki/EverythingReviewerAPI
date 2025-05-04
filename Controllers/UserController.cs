
using reviews4everything.DTOs;
using reviews4everything.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using reviews4everything.Data;

namespace reviews4everything.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfiles()
        {
            var userId = User.FindFirst("id")?.Value;

            Console.WriteLine(userId);
            //todo add pagination
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));
            var user_reviwes = await _context.Reviews.Include(r=>r.Item).Where(r => r.Uid == userId).Select(r=>new ReviewDTO(r)).ToListAsync();

            if (user == null) { return NotFound(); }

           

            return Ok(new ProfileDTO(user, user_reviwes));

        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile( [FromBody] ProfileDTO model)
        {
            Console.WriteLine(model.Username);
            var userId = User.FindFirst("id")?.Value;
            var user = await _context.Users.Include(u => u.ReviewsAdded).FirstOrDefaultAsync(x => x.Id.Equals(userId));
            if (user == null) { return NotFound(); }

            //update profile
            var updateResult = await UpdateUserProfile(user, model);

            if(!updateResult)
            {
                return BadRequest(new { message = "username already taken" });
            }

           

            await _context.SaveChangesAsync();

            return Ok(new {message = "successfully upadted profile"});

        }

        //updates user fields to match profile fields if profile field is not null
        //doesnt save to db
        //returns true if successful
        private async Task<bool> UpdateUserProfile(AppUser user, ProfileDTO newProfile)
        {
            //pfpUrl can be null cuz dah
            if (newProfile.PfpUrl != user.PfpUrl)
            {
                user.PfpUrl = newProfile.PfpUrl;
            }

            
            if (newProfile.Username != null && user.UserName != null)
            {
                //if user didnt submit the same name
                if (newProfile.Username.ToUpper() != user.UserName.ToUpper())
                {
                    //return immediately if username taken
                    if (!(await ValidateUsernameIsFree(newProfile.Username))) { return false; }

                    //updates username and normalized name
                    user.UserName = newProfile.Username;
                    user.NormalizedUserName = newProfile.Username.ToUpperInvariant();
                }

                //if user submitted the same name with different case
                else if (newProfile.Username != user.UserName)
                {
                    user.UserName = newProfile.Username;
                }
                
            }

            return true;

        }

        //checks username.toUpper() is not already someone's normalized username
        //returns true if username avaliable
        private async Task<bool> ValidateUsernameIsFree(string? username)
        {
            Console.WriteLine($"checking if {username} is taken");
            if (username == null) { return true; }



            var userWithUsername = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == username.ToUpper());


            Console.WriteLine($"userWithUsername is  {userWithUsername}");
            return userWithUsername == null;
        }


    }
}
