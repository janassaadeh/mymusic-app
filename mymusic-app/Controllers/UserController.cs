using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mymusic_app.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace mymusic_app.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // ---------------- Helper ----------------
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID not found in token");

            return Guid.Parse(userIdClaim);
        }

        // ---------------- GET ALL USERS ----------------
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // ---------------- GET USER BY ID ----------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // ---------------- EDIT PROFILE ----------------
        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileDto dto)
        {
            var userId = GetCurrentUserId();

            try
            {
                await _userService.EditProfileAsync(userId, dto.FirstName, dto.LastName, dto.Bio);
                return NoContent(); // 204
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ---------------- PLAY SONG ----------------
        [HttpPost("play/{songId}")]
        public async Task<IActionResult> PlaySong(Guid songId)
        {
            var userId = GetCurrentUserId();
            await _userService.PlaySongAsync(userId, songId);
            return Ok(new { message = "Song played" });
        }

        // ---------------- LIKE / UNLIKE ----------------
        [HttpPost("like/{songId}")]
        public async Task<IActionResult> LikeSong(Guid songId)
        {
            var userId = GetCurrentUserId();
            await _userService.LikeSongAsync(userId, songId);
            return Ok(new { message = "Song liked" });
        }

        [HttpDelete("like/{songId}")]
        public async Task<IActionResult> UnlikeSong(Guid songId)
        {
            var userId = GetCurrentUserId();
            await _userService.UnlikeSongAsync(userId, songId);
            return Ok(new { message = "Song unliked" });
        }

        // ---------------- FOLLOW / UNFOLLOW USERS ----------------
        [HttpPost("follow/{followUserId}")]
        public async Task<IActionResult> FollowUser(Guid followUserId)
        {
            var userId = GetCurrentUserId();
            await _userService.FollowUserAsync(userId, followUserId);
            return Ok(new { message = "User followed" });
        }

        [HttpDelete("follow/{followUserId}")]
        public async Task<IActionResult> UnfollowUser(Guid followUserId)
        {
            var userId = GetCurrentUserId();
            await _userService.UnfollowUserAsync(userId, followUserId);
            return Ok(new { message = "User unfollowed" });
        }

        // ---------------- FOLLOW / UNFOLLOW ARTISTS ----------------
        [HttpPost("follow-artist/{artistId}")]
        public async Task<IActionResult> FollowArtist(Guid artistId)
        {
            var userId = GetCurrentUserId();
            await _userService.FollowArtistAsync(userId, artistId);
            return Ok(new { message = "Artist followed" });
        }

        [HttpDelete("follow-artist/{artistId}")]
        public async Task<IActionResult> UnfollowArtist(Guid artistId)
        {
            var userId = GetCurrentUserId();
            await _userService.UnfollowArtistAsync(userId, artistId);
            return Ok(new { message = "Artist unfollowed" });
        }

        // ---------------- FOLLOW / UNFOLLOW PLAYLISTS ----------------
        [HttpPost("follow-playlist/{playlistId}")]
        public async Task<IActionResult> FollowPlaylist(Guid playlistId)
        {
            var userId = GetCurrentUserId();
            await _userService.FollowPlaylistAsync(userId, playlistId);
            return Ok(new { message = "Playlist followed" });
        }

        [HttpDelete("follow-playlist/{playlistId}")]
        public async Task<IActionResult> UnfollowPlaylist(Guid playlistId)
        {
            var userId = GetCurrentUserId();
            await _userService.UnfollowPlaylistAsync(userId, playlistId);
            return Ok(new { message = "Playlist unfollowed" });
        }

        // ---------------- FETCH CURRENT USER COLLECTIONS ----------------
        [HttpGet("liked-songs")]
        public async Task<IActionResult> LikedSongs()
        {
            var userId = GetCurrentUserId();
            var songs = await _userService.GetLikedSongsAsync(userId);
            return Ok(songs);
        }

        [HttpGet("recently-played")]
        public async Task<IActionResult> RecentlyPlayed()
        {
            var userId = GetCurrentUserId();
            var songs = await _userService.GetRecentlyPlayedAsync(userId);
            return Ok(songs);
        }

        [HttpGet("followed-artists")]
        public async Task<IActionResult> FollowedArtists()
        {
            var userId = GetCurrentUserId();
            var artists = await _userService.GetFollowedArtistsAsync(userId);
            return Ok(artists);
        }

        [HttpGet("followed-albums")]
        public async Task<IActionResult> FollowedAlbums()
        {
            var userId = GetCurrentUserId();
            var albums = await _userService.GetFollowedAlbumsAsync(userId);
            return Ok(albums);
        }

        [HttpGet("followed-playlists")]
        public async Task<IActionResult> FollowedPlaylists()
        {
            var userId = GetCurrentUserId();
            var playlists = await _userService.GetFollowedPlaylistsAsync(userId);
            return Ok(playlists);
        }

        [HttpGet("followers")]
        public async Task<IActionResult> Followers()
        {
            var userId = GetCurrentUserId();
            var followers = await _userService.GetFollowersAsync(userId);
            return Ok(followers);
        }

        [HttpGet("following")]
        public async Task<IActionResult> Following()
        {
            var userId = GetCurrentUserId();
            var following = await _userService.GetFollowingAsync(userId);
            return Ok(following);
        }

        [HttpGet("following-activity")]
        public async Task<IActionResult> FollowingActivity()
        {
            var userId = GetCurrentUserId();
            var feed = await _userService.GetFollowedUsersRecentPlaysAsync(userId);
            return Ok(feed);
        }
    }

    public class EditProfileDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Bio { get; set; }
    }
}
