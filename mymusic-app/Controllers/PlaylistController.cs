using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mymusic_app.Services;
using mymusic_app.Models;

namespace mymusic_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PlaylistsController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistsController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        // GET api/playlists
        [HttpGet]
        public async Task<IActionResult> GetAllPlaylists()
        {
            var playlists = await _playlistService.GetAllPlaylistsAsync();
            return Ok(playlists);
        }

        // GET api/playlists/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var playlist = await _playlistService.GetByIdAsync(id);
            if (playlist == null)
                return NotFound();
            return Ok(playlist);
        }

        // POST api/playlists
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlaylistDto dto)
        {
            // Get the logged-in user's ID from JWT claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User ID not found in token");

            var userId = Guid.Parse(userIdClaim);

            // Create a new playlist
            var playlist = new Playlist
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                OwnerId = userId,
                Description = dto.Description ?? "", // default to empty string if null
                Songs = new List<PlaylistSong>(),    // initialize empty list
                Followers = new List<PlaylistFollow>() // initialize empty list
            };

            // Add initial songs if provided
            if (dto.SongIds != null)
            {
                for (int i = 0; i < dto.SongIds.Count; i++)
                {
                    playlist.Songs.Add(new PlaylistSong
                    {
                        PlaylistId = playlist.Id,
                        SongId = dto.SongIds[i],
                        Position = i + 1
                    });
                }
            }

            // Save playlist
            await _playlistService.CreatePlaylistAsync(playlist);

            return CreatedAtAction(nameof(Details), new { id = playlist.Id }, playlist);
        }


        // DELETE api/playlists/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _playlistService.DeletePlaylistAsync(id);
            return NoContent();
        }

        // POST api/playlists/{playlistId}/songs
        [HttpPost("{playlistId}/songs")]
        public async Task<IActionResult> AddSong(Guid playlistId, [FromQuery] Guid songId, [FromQuery] int position)
        {
            await _playlistService.AddSongAsync(playlistId, songId, position);
            return NoContent();
        }

        // DELETE api/playlists/{playlistId}/songs/{songId}
        [HttpDelete("{playlistId}/songs/{songId}")]
        public async Task<IActionResult> RemoveSong(Guid playlistId, Guid songId)
        {
            await _playlistService.RemoveSongAsync(playlistId, songId);
            return NoContent();
        }

        // PUT api/playlists/{playlistId}/reorder
        [HttpPut("{playlistId}/reorder")]
        public async Task<IActionResult> Reorder(Guid playlistId, [FromBody] List<Guid> songIds)
        {
            await _playlistService.ReorderSongsAsync(playlistId, songIds);
            return NoContent();
        }
    }
    public class CreatePlaylistDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } // optional
        public List<Guid> SongIds { get; set; } = new();
    }


}
