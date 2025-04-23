using Microsoft.AspNetCore.Mvc;
using EventFinderAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace EventFinderAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SecureUrlProvider : Controller
    {
        private readonly S3UploadService _S3UploadService;
        
        public SecureUrlProvider(S3UploadService S3UploadService)
        {
            _S3UploadService = S3UploadService;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string key)
        {
            string secure_url = _S3UploadService.GeneratePreSignedUploadUrl(key.ToString());

            if (string.IsNullOrEmpty(secure_url))
            {
                return StatusCode(500);
            }

            return Ok(new { secure_url });
        }

    }
}
