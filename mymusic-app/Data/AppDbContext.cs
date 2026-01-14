using Microsoft.EntityFrameworkCore;
using mymusic_app.Models;

namespace mymusic_app.Controllers.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Playlist> Playlists { get; set; }

        // Join tables
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<UserArtistFollow> UserArtistFollows { get; set; }
        public DbSet<UserAlbumFollow> UserAlbumFollows { get; set; }
        public DbSet<UserSongLike> UserSongLikes { get; set; }
        public DbSet<UserSongPlay> UserSongPlays { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<PlaylistFollow> PlaylistFollows { get; set; }
        public DbSet<ArtistGenre> ArtistGenres { get; set; }
        public DbSet<SongGenre> SongGenres { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // -----------------------
            // User ↔ User (Followers)
            // -----------------------
            builder.Entity<UserFollow>()
                .HasKey(uf => new { uf.FollowerId, uf.FollowingId });

            builder.Entity<UserFollow>()
                .HasOne(uf => uf.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(uf => uf.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserFollow>()
                .HasOne(uf => uf.Following)
                .WithMany(u => u.Followers)
                .HasForeignKey(uf => uf.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------
            // User ↔ Artist
            // -----------------------
            builder.Entity<UserArtistFollow>()
                .HasKey(ua => new { ua.UserId, ua.ArtistId });

            builder.Entity<UserArtistFollow>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.FollowedArtists)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserArtistFollow>()
                .HasOne(ua => ua.Artist)
                .WithMany(a => a.Followers)
                .HasForeignKey(ua => ua.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------
            // User ↔ Album
            // -----------------------
            builder.Entity<UserAlbumFollow>()
                .HasKey(ua => new { ua.UserId, ua.AlbumId });

            builder.Entity<UserAlbumFollow>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.FollowedAlbums)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserAlbumFollow>()
                .HasOne(ua => ua.Album)
                .WithMany(a => a.Followers)
                .HasForeignKey(ua => ua.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------
            // User ↔ Song Likes
            // -----------------------
            builder.Entity<UserSongLike>()
                .HasKey(us => new { us.UserId, us.SongId });

            builder.Entity<UserSongLike>()
                .HasOne(us => us.User)
                .WithMany(u => u.LikedSongs)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserSongLike>()
                .HasOne(us => us.Song)
                .WithMany(s => s.Likes)
                .HasForeignKey(us => us.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------
            // User ↔ Song Plays
            // -----------------------
            builder.Entity<UserSongPlay>()
                .HasKey(usp => usp.Id);

            builder.Entity<UserSongPlay>()
                .HasOne(usp => usp.User)
                .WithMany(u => u.SongPlays)
                .HasForeignKey(usp => usp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserSongPlay>()
                .HasOne(usp => usp.Song)
                .WithMany(s => s.Plays)
                .HasForeignKey(usp => usp.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserSongPlay>()
                .HasIndex(usp => new { usp.UserId, usp.PlayedAt });

            // -----------------------
            // Playlist ↔ Song
            // -----------------------
            builder.Entity<PlaylistSong>()
                .HasKey(ps => new { ps.PlaylistId, ps.SongId });

            builder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Playlist)
                .WithMany(p => p.Songs)
                .HasForeignKey(ps => ps.PlaylistId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Song)
                .WithMany(s => s.Playlists)
                .HasForeignKey(ps => ps.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------
            // Playlist Followers
            // -----------------------
            builder.Entity<PlaylistFollow>()
                .HasKey(pf => new { pf.UserId, pf.PlaylistId });

            builder.Entity<PlaylistFollow>()
                .HasOne(pf => pf.User)
                .WithMany(u => u.FollowedPlaylists)
                .HasForeignKey(pf => pf.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PlaylistFollow>()
                .HasOne(pf => pf.Playlist)
                .WithMany(p => p.Followers)
                .HasForeignKey(pf => pf.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------
            // Artist ↔ Genre
            // -----------------------
            builder.Entity<ArtistGenre>()
                .HasKey(ag => new { ag.ArtistId, ag.GenreId });

            builder.Entity<ArtistGenre>()
                .HasOne(ag => ag.Artist)
                .WithMany(a => a.Genres)
                .HasForeignKey(ag => ag.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ArtistGenre>()
                .HasOne(ag => ag.Genre)
                .WithMany(g => g.Artists)
                .HasForeignKey(ag => ag.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------
            // Song ↔ Genre
            // -----------------------
            builder.Entity<SongGenre>()
                .HasKey(sg => new { sg.SongId, sg.GenreId });

            builder.Entity<SongGenre>()
                .HasOne(sg => sg.Song)
                .WithMany(s => s.Genres)
                .HasForeignKey(sg => sg.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SongGenre>()
                .HasOne(sg => sg.Genre)
                .WithMany(g => g.Songs)
                .HasForeignKey(sg => sg.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------
            // Song ↔ Album / Artist
            // -----------------------
            builder.Entity<Song>()
                .HasOne(s => s.Album)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Song>()
                .HasOne(s => s.Artist)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.ArtistId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------
            // Indexes
            // -----------------------
            builder.Entity<Song>().HasIndex(s => s.Title);
            builder.Entity<Artist>().HasIndex(a => a.Name);
            builder.Entity<Album>().HasIndex(a => a.Title);
            builder.Entity<Playlist>().HasIndex(p => p.Name);
        }
    }
}
