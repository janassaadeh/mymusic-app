namespace mymusic_app.DTOs
{
    public class ArtistDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public List<GenreDto> Genres { get; set; } = new();
        public List<AlbumDto> Albums { get; set; } = new();
        public List<SongDto> Songs { get; set; } = new();
        public int FollowerCount { get; set; }
    }

    public class AlbumDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string CoverImageUrl { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int SongCount { get; set; }
    }

    public class SongDto
    {
        public Guid Id { get; set; }
        public string DeezerTrackId { get; set; }
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class GenreDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
