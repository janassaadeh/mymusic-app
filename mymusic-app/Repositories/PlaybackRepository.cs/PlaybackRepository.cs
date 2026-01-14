using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using mymusic_app.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mymusic_app.Repositories
{
    public class PlaybackRepository : IPlaybackRepository
    {
        private readonly string _connectionString;

        public PlaybackRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // ============================================================
        // RECENTLY PLAYED
        // ============================================================
        public async Task<IEnumerable<Song>> GetRecentlyPlayedSongsAsync(Guid userId)
        {
            var songs = new List<Song>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                SELECT TOP 20
                    s.Id,
                    s.Title,
                    s.Duration,

                    a.Id AS ArtistId,
                    a.Name AS ArtistName,

                    al.Id AS AlbumId,
                    al.Title AS AlbumTitle,
                    al.CoverImageUrl
                FROM UserSongPlays p
                JOIN Songs s ON p.SongId = s.Id
                LEFT JOIN Artists a ON s.ArtistId = a.Id
                LEFT JOIN Albums al ON s.AlbumId = al.Id
                WHERE p.UserId = @UserId
                ORDER BY p.PlayedAt DESC
            ", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                songs.Add(new Song
                {
                    Id = reader.GetGuid(0),
                    Title = reader.GetString(1),
                    Duration = reader.GetTimeSpan(2),

                    Artist = reader.IsDBNull(3) ? null : new Artist
                    {
                        Id = reader.GetGuid(3),
                        Name = reader.GetString(4)
                    },

                    Album = reader.IsDBNull(5) ? null : new Album
                    {
                        Id = reader.GetGuid(5),
                        Title = reader.GetString(6),
                        CoverImageUrl = reader.IsDBNull(7) ? null : reader.GetString(7)
                    }
                });
            }

            return songs;
        }

        public async Task DeleteOldRecentlyPlayedAsync(Guid userId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                DELETE FROM UserSongPlays
                WHERE UserId = @UserId
                  AND PlayedAt < DATEADD(day, -1, GETDATE())
            ", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);
            await cmd.ExecuteNonQueryAsync();
        }

        // ============================================================
        // FOLLOWED USERS RECENT PLAYS
        // ============================================================
        public async Task<IEnumerable<(User User, Song Song)>> GetFollowedUsersRecentPlaysAsync(Guid userId)
        {
            var result = new List<(User, Song)>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                SELECT TOP 20
                    u.Id,
                    u.FirstName,
                    u.LastName,

                    s.Id,
                    s.Title,
                    s.Duration,

                    a.Id AS ArtistId,
                    a.Name AS ArtistName,

                    al.Id AS AlbumId,
                    al.Title AS AlbumTitle,
                    al.CoverImageUrl
                FROM Followings f
                JOIN Users u ON f.FollowingId = u.Id
                JOIN UserSongPlays p ON p.UserId = u.Id
                JOIN Songs s ON s.Id = p.SongId
                LEFT JOIN Artists a ON s.ArtistId = a.Id
                LEFT JOIN Albums al ON s.AlbumId = al.Id
                WHERE f.UserId = @UserId
                ORDER BY p.PlayedAt DESC
            ", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var user = new User
                {
                    Id = reader.GetGuid(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2)
                };

                var song = new Song
                {
                    Id = reader.GetGuid(3),
                    Title = reader.GetString(4),
                    Duration = reader.GetTimeSpan(5),

                    Artist = reader.IsDBNull(6) ? null : new Artist
                    {
                        Id = reader.GetGuid(6),
                        Name = reader.GetString(7)
                    },

                    Album = reader.IsDBNull(8) ? null : new Album
                    {
                        Id = reader.GetGuid(8),
                        Title = reader.GetString(9),
                        CoverImageUrl = reader.IsDBNull(10) ? null : reader.GetString(10)
                    }
                };

                result.Add((user, song));
            }

            return result;
        }

        // ============================================================
        // SONG PLAYS
        // ============================================================
        public async Task RecordSongPlayAsync(Guid userId, Guid songId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                INSERT INTO UserSongPlays (Id, UserId, SongId, PlayedAt)
                VALUES (NEWID(), @UserId, @SongId, GETDATE())
            ", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@SongId", songId);

            await cmd.ExecuteNonQueryAsync();
        }

        // ============================================================
        // LIKES
        // ============================================================
        public async Task LikeSongAsync(Guid userId, Guid songId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                IF NOT EXISTS (
                    SELECT 1 FROM UserSongLikes
                    WHERE UserId = @UserId AND SongId = @SongId
                )
                INSERT INTO UserSongLikes (Id, UserId, SongId)
                VALUES (NEWID(), @UserId, @SongId)
            ", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@SongId", songId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UnlikeSongAsync(Guid userId, Guid songId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                DELETE FROM UserSongLikes
                WHERE UserId = @UserId AND SongId = @SongId
            ", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@SongId", songId);

            await cmd.ExecuteNonQueryAsync();
        }

        // ============================================================
        // FOLLOW USERS
        // ============================================================
        public async Task FollowUserAsync(Guid userId, Guid followUserId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                IF NOT EXISTS (
                    SELECT 1 FROM Followings
                    WHERE UserId = @UserId AND FollowingId = @FollowId
                )
                INSERT INTO Followings (Id, UserId, FollowingId)
                VALUES (NEWID(), @UserId, @FollowId)
            ", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@FollowId", followUserId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UnfollowUserAsync(Guid userId, Guid followUserId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                DELETE FROM Followings
                WHERE UserId = @UserId AND FollowingId = @FollowId
            ", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@FollowId", followUserId);

            await cmd.ExecuteNonQueryAsync();
        }

        // ============================================================
        // FOLLOW ARTISTS
        // ============================================================
        public async Task FollowArtistAsync(Guid userId, Guid artistId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                IF NOT EXISTS (
                    SELECT 1 FROM UserArtistFollows
                    WHERE UserId = @UserId AND ArtistId = @ArtistId
                )
                INSERT INTO UserArtistFollows (Id, UserId, ArtistId)
                VALUES (NEWID(), @UserId, @ArtistId)
            ", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@ArtistId", artistId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UnfollowArtistAsync(Guid userId, Guid artistId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                DELETE FROM UserArtistFollows
                WHERE UserId = @UserId AND ArtistId = @ArtistId
            ", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@ArtistId", artistId);

            await cmd.ExecuteNonQueryAsync();
        }

        // ============================================================
        // FOLLOW PLAYLISTS
        // ============================================================
        public async Task FollowPlaylistAsync(Guid userId, Guid playlistId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                IF NOT EXISTS (
                    SELECT 1 FROM UserPlaylistFollows
                    WHERE UserId = @UserId AND PlaylistId = @PlaylistId
                )
                INSERT INTO UserPlaylistFollows (Id, UserId, PlaylistId)
                VALUES (NEWID(), @UserId, @PlaylistId)
            ", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@PlaylistId", playlistId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UnfollowPlaylistAsync(Guid userId, Guid playlistId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                DELETE FROM UserPlaylistFollows
                WHERE UserId = @UserId AND PlaylistId = @PlaylistId
            ", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@PlaylistId", playlistId);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
