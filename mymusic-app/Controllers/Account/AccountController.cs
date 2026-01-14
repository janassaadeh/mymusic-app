using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mymusic_app.Models;
using mymusic_app.Services;

namespace mymusic_app.Controllers
{
    [Authorize] // Require login
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID not found");

            return Guid.Parse(userIdClaim);
        }

        // ---------------- USER DASHBOARD ----------------
        public async Task<IActionResult> Dashboard()
        {
            var userId = GetCurrentUserId();

            // Fetch all relevant data for the dashboard
            var user = await _userService.GetProfileAsync(userId);
            var likedSongs = await _userService.GetLikedSongsAsync(userId);
            var recentlyPlayed = await _userService.GetRecentlyPlayedAsync(userId);
            var followedArtists = await _userService.GetFollowedArtistsAsync(userId);
            var followedPlaylists = await _userService.GetFollowedPlaylistsAsync(userId);

            var followersCount = await _userService.GetFollowersCountAsync(userId);
            var followingCount = await _userService.GetFollowingCountAsync(userId);

            var model = new UserDashboardViewModel
            {
                User = user,
                LikedSongs = likedSongs?.ToList() ?? new List<Song>(),
                RecentlyPlayed = recentlyPlayed?.ToList() ?? new List<Song>(),
                FollowedArtists = followedArtists?.ToList() ?? new List<Artist>(),
                FollowedPlaylists = followedPlaylists?.ToList() ?? new List<Playlist>(),
                FollowersCount = followersCount,
                FollowingCount = followingCount
            };

            return View(model); // Returns Views/Account/Dashboard.cshtml
        }

        // ---------------- EDIT PROFILE ----------------
        public async Task<IActionResult> Edit()
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetProfileAsync(userId);

            if (user == null) return NotFound();

            // Prefill the form
            var model = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio
            };

            return View(model); // Passes prefilled data to the Razor form
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = GetCurrentUserId();
            await _userService.EditProfileAsync(userId, model.FirstName, model.LastName, model.Bio);

            return RedirectToAction("Dashboard");
        }
    }

    // ---------------- VIEW MODELS ----------------
    public class UserDashboardViewModel
    {
        public User User { get; set; } = null!;
        public List<Song> LikedSongs { get; set; } = new();
        public List<Song> RecentlyPlayed { get; set; } = new();
        public List<Artist> FollowedArtists { get; set; } = new();
        public List<Playlist> FollowedPlaylists { get; set; } = new();
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
    }

    public class EditProfileViewModel
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string FirstName { get; set; } = null!;

        [System.ComponentModel.DataAnnotations.Required]
        public string LastName { get; set; } = null!;

        public string? Bio { get; set; }
    }

}
