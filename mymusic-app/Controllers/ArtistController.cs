using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mymusic_app.DTOs;
using mymusic_app.Models;
using mymusic_app.Services;

namespace mymusic_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ArtistController : ControllerBase
    {
        private readonly IArtistService _artistService;

        public ArtistController(IArtistService artistService)
        {
            _artistService = artistService;
        }

        // GET api/artist
        [HttpGet]
        public async Task<IActionResult> GetAllArtists()
        {
            var artists = await _artistService.GetAllAsync();

            var dtos = artists.Select(MapToDto);

            return Ok(dtos);
        }

        // GET api/artist/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var artist = await _artistService.GetArtistByIdAsync(id);
            if (artist == null)
                return NotFound();

            var dto = MapToDto(artist);
            return Ok(dto);
        }

        // GET api/artist/{id}/topsongs
        [HttpGet("{id}/topsongs")]
        public async Task<IActionResult> TopSongs(Guid id)
        {
            var songs = await _artistService.GetTopSongsAsync(id);
            var dtos = songs.Select(s => new SongDto
            {
                Id = s.Id,
                DeezerTrackId = s.DeezerTrackId,
                Title = s.Title,
                Duration = s.Duration
            });
            return Ok(dtos);
        }

        // GET api/artist/{id}/albums
        [HttpGet("{id}/albums")]
        public async Task<IActionResult> Albums(Guid id)
        {
            var albums = await _artistService.GetAlbumsWithSongsAsync(id);
            var dtos = albums.Select(a => new AlbumDto
            {
                Id = a.Id,
                Title = a.Title,
                CoverImageUrl = a.CoverImageUrl,
                ReleaseDate = a.ReleaseDate,
                SongCount = a.Songs?.Count ?? 0
            });
            return Ok(dtos);
        }

        // GET api/artist/{id}/similar
        [HttpGet("{id}/similar")]
        public async Task<IActionResult> Similar(Guid id)
        {
            var artists = await _artistService.GetSimilarArtistsAsync(id);
            var dtos = artists.Select(MapToDto);
            return Ok(dtos);
        }

        // ------------------- Helper -------------------
        private ArtistDto MapToDto(Artist a)
        {
            return new ArtistDto
            {
                Id = a.Id,
                Name = a.Name,
                ImageUrl = a.ImageUrl,
                Genres = a.Genres?.Select(g => new GenreDto
                {
                    Id = g.GenreId,
                    Name = g.Genre?.Name ?? "Unknown"
                }).ToList() ?? new List<GenreDto>(),
                Albums = a.Albums?.Select(al => new AlbumDto
                {
                    Id = al.Id,
                    Title = al.Title,
                    CoverImageUrl = al.CoverImageUrl,
                    ReleaseDate = al.ReleaseDate,
                    SongCount = al.Songs?.Count ?? 0
                }).ToList() ?? new List<AlbumDto>(),
                Songs = a.Songs?.Select(s => new SongDto
                {
                    Id = s.Id,
                    DeezerTrackId = s.DeezerTrackId,
                    Title = s.Title,
                    Duration = s.Duration
                }).ToList() ?? new List<SongDto>(),
                FollowerCount = a.Followers?.Count ?? 0
            };
        }
    }
}
