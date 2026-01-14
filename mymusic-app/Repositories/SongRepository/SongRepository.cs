using Microsoft.EntityFrameworkCore;
using mymusic_app.Controllers.Data;
using mymusic_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mymusic_app.Repositories
{
    public class SongRepository : ISongRepository
    {
        private readonly AppDbContext _db;

        public SongRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Song>> GetTopSongsByGenreAsync(Guid genreId, int limit = 20)
        {
            var songsQuery = _db.Songs
                .Include(s => s.Album)
                    .ThenInclude(a => a.Artist)
                .Include(s => s.Artist)
                .Where(s => s.Genres.Any(g => g.GenreId == genreId));

            var topSongs = await songsQuery
                .OrderByDescending(s => s.Plays.Count) 
                .Take(limit)
                .ToListAsync();

            return topSongs;
        }


        public async Task<Song> AddAsync(Song song)
        {
            _db.Songs.Add(song);
            await _db.SaveChangesAsync();
            return song;
        }

        public async Task<IEnumerable<Song>> GetAllAsync()
        {
            return await _db.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .Include(s => s.Genres)
                .ToListAsync();
        }

        public async Task<Song> GetByIdAsync(Guid id)
        {
            return await _db.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .Include(s => s.Genres)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(Song song)
        {
            _db.Songs.Update(song);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var song = await _db.Songs.FindAsync(id);
            if (song != null)
            {
                _db.Songs.Remove(song);
                await _db.SaveChangesAsync();
            }
        }
    }
}
