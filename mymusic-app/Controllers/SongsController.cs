using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mymusic_app.Services;

namespace mymusic_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class SongsController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongsController(ISongService songService)
        {
            _songService = songService;
        }

        // GET api/songs
        [HttpGet]
        public async Task<IActionResult> GetAllSongs()
        {
            var songs = await _songService.GetAllAsync();
            return Ok(songs);
        }

        // GET api/songs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSongById(Guid id)
        {
            var song = await _songService.GetByIdAsync(id);
            if (song == null)
                return NotFound();
            return Ok(song);
        }

        // GET api/songs/top-by-genre?genreId={genreId}
        [HttpGet("top-by-genre")]
        public async Task<IActionResult> TopByGenre([FromQuery] Guid genreId)
        {
            var songs = await _songService.GetTopSongsByGenreAsync(genreId);
            return Ok(songs);
        }
    }
}
