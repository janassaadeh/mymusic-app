using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mymusic_app.Services;

namespace mymusic_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FeedController : ControllerBase
    {
        private readonly IUserService _userService;

        public FeedController(IUserService userService)
        {
            _userService = userService;
        }

        // GET api/feed/following-activity?userId={userId}
        [HttpGet("following-activity")]
        public async Task<IActionResult> FollowingActivity([FromQuery] Guid userId)
        {
            var feed = await _userService.GetFollowedUsersRecentPlaysAsync(userId);
            return Ok(feed);
        }
    }
}
