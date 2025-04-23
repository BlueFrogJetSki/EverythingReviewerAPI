using EventFinderAPI.Data;
using EventFinderAPI.DTOs;
using EventFinderAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EventFinderAPI.Controllers
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
            var user = await _context.Users.Include(u => u.ReviewsAdded).FirstOrDefaultAsync(x => x.Id.Equals(userId));

            if (user == null) { return NotFound(); }

            return Ok(new ProfileDTO(user));

        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile( [FromBody] ProfileDTO model)
        {
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
            if (newProfile.PfpUrl != user.pfpUrl)
            {
                user.pfpUrl = newProfile.PfpUrl;
            }

            
            if (newProfile.Username != null && user.UserName != null)
            {
                //if user didnt submit the same name
                if (newProfile.Username.ToUpper() != user.UserName.ToUpper())
                {
                    //return immediately if username taken
                    if ((await ValidateUsernameIsNotTaken(newProfile.Username))) { return false; }

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
        private async Task<bool> ValidateUsernameIsNotTaken(string? username)
        {
            if(username == null) { return true; }

            var userWithUsername = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName != username.ToUpper());

            return userWithUsername == null;
        }


    }
}
