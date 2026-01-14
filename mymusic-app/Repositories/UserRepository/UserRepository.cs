using Microsoft.EntityFrameworkCore;
using mymusic_app.Controllers.Data;
using mymusic_app.Models;

namespace mymusic_app.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        // ---------------- USERS ----------------

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
            => await _db.Users.ToListAsync();

        public async Task<User> GetByIdAsync(Guid id)
            => await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User> GetByEmailAsync(string email)
            => await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task UpdateAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        // ---------------- FOLLOWERS / FOLLOWING ----------------

        public async Task<int> GetFollowersCountAsync(Guid userId)
            => await _db.UserFollows.CountAsync(f => f.FollowingId == userId);

        public async Task<int> GetFollowingCountAsync(Guid userId)
            => await _db.UserFollows.CountAsync(f => f.FollowerId == userId);

        public async Task<IEnumerable<User>> GetFollowersAsync(Guid userId)
            => await _db.UserFollows
                .Where(f => f.FollowingId == userId)
                .Select(f => f.Follower)
                .ToListAsync();

        public async Task<IEnumerable<User>> GetFollowingAsync(Guid userId)
            => await _db.UserFollows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.Following)
                .ToListAsync();

        // ---------------- SONGS ----------------

        public async Task<IEnumerable<Song>> GetTopPlayedSongsAsync(Guid userId, int limit = 20)
            => await _db.UserSongPlays
                .Where(p => p.UserId == userId)
                .GroupBy(p => p.Song)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(limit)
                .ToListAsync();

        public async Task<int> GetLikesCountAsync(Guid userId)
            => await _db.UserSongLikes.CountAsync(l => l.UserId == userId);

        public async Task<IEnumerable<Song>> GetLikedSongsAsync(Guid userId)
    => await _db.UserSongLikes
        .Where(l => l.UserId == userId)
        .Include(l => l.Song)
            .ThenInclude(s => s.Artist)
        .Include(l => l.Song)
            .ThenInclude(s => s.Album)
        .Select(l => l.Song)
        .ToListAsync();


        public async Task LikeSongAsync(Guid userId, Guid songId)
        {
            if (!await _db.UserSongLikes.AnyAsync(l => l.UserId == userId && l.SongId == songId))
            {
                _db.UserSongLikes.Add(new UserSongLike
                {
                    UserId = userId,
                    SongId = songId
                });
                await _db.SaveChangesAsync();
            }
        }

        public async Task UnlikeSongAsync(Guid userId, Guid songId)
        {
            var like = await _db.UserSongLikes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.SongId == songId);

            if (like != null)
            {
                _db.UserSongLikes.Remove(like);
                await _db.SaveChangesAsync();
            }
        }

        // ---------------- ARTISTS / ALBUMS ----------------

        public async Task<IEnumerable<Artist>> GetFollowedArtistsAsync(Guid userId)
            => await _db.UserArtistFollows
                .Where(f => f.UserId == userId)
                .Select(f => f.Artist)
                .ToListAsync();

        public async Task<IEnumerable<Album>> GetFollowedAlbumsAsync(Guid userId)
            => await _db.UserAlbumFollows
                .Where(f => f.UserId == userId)
                .Select(f => f.Album)
                .ToListAsync();

        public async Task FollowArtistAsync(Guid userId, Guid artistId)
        {
            if (!await _db.UserArtistFollows.AnyAsync(f => f.UserId == userId && f.ArtistId == artistId))
            {
                _db.UserArtistFollows.Add(new UserArtistFollow
                {
                    UserId = userId,
                    ArtistId = artistId
                });
                await _db.SaveChangesAsync();
            }
        }

        public async Task UnfollowArtistAsync(Guid userId, Guid artistId)
        {
            var follow = await _db.UserArtistFollows
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ArtistId == artistId);

            if (follow != null)
            {
                _db.UserArtistFollows.Remove(follow);
                await _db.SaveChangesAsync();
            }
        }

        // ---------------- PLAYLISTS ----------------

        public async Task<IEnumerable<Playlist>> GetUserPlaylistsAsync(Guid userId)
            => await _db.Playlists
                .Where(p => p.OwnerId == userId)
                .ToListAsync();

        public async Task<IEnumerable<Playlist>> GetFollowedPlaylistsAsync(Guid userId)
            => await _db.PlaylistFollows
                .Where(f => f.UserId == userId)
                .Select(f => f.Playlist)
                .ToListAsync();

        public async Task FollowPlaylistAsync(Guid userId, Guid playlistId)
        {
            if (!await _db.PlaylistFollows.AnyAsync(f => f.UserId == userId && f.PlaylistId == playlistId))
            {
                _db.PlaylistFollows.Add(new PlaylistFollow
                {
                    UserId = userId,
                    PlaylistId = playlistId
                });
                await _db.SaveChangesAsync();
            }
        }

        public async Task UnfollowPlaylistAsync(Guid userId, Guid playlistId)
        {
            var follow = await _db.PlaylistFollows
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PlaylistId == playlistId);

            if (follow != null)
            {
                _db.PlaylistFollows.Remove(follow);
                await _db.SaveChangesAsync();
            }
        }

        // ---------------- FOLLOW USERS ----------------

        public async Task FollowUserAsync(Guid userId, Guid followUserId)
        {
            if (!await _db.UserFollows.AnyAsync(f => f.FollowerId == userId && f.FollowingId == followUserId))
            {
                _db.UserFollows.Add(new UserFollow
                {
                    FollowerId = userId,
                    FollowingId = followUserId
                });
                await _db.SaveChangesAsync();
            }
        }

        public async Task UnfollowUserAsync(Guid userId, Guid followUserId)
        {
            var follow = await _db.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerId == userId && f.FollowingId == followUserId);

            if (follow != null)
            {
                _db.UserFollows.Remove(follow);
                await _db.SaveChangesAsync();
            }
        }
    }
}
