using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mymusic_app.Services;

namespace mymusic_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController] // Enables API conventions
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AlbumsController : ControllerBase
    {
        private readonly IAlbumService _albumService;

        public AlbumsController(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        // GET api/albums
        [HttpGet]
        public async Task<IActionResult> GetAllAlbums()
        {
            var albums = await _albumService.GetAllAsync();
            return Ok(albums);
        }

        // GET api/albums/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var album = await _albumService.GetByIdAsync(id);
            if (album == null)
                return NotFound();
            return Ok(album);
        }

        // GET api/albums/{id}/songs
        [HttpGet("{id}/songs")]
        public async Task<IActionResult> Songs(Guid id)
        {
            var songs = await _albumService.GetAlbumSongsAsync(id);
            return Ok(songs);
        }
    }
}
