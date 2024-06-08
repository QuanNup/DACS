
namespace SharedClassLibrary.Entities
{
    public class Album
    {
        public int AlbumId { get; set; }
        public string? AlbumName { get; set; }
        public string? AlbumDescription { get; set; }
        public string? AlbumImage { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public List<Song> Song { get; set; }
        public int ArtistId { get; set; }
        public virtual Artist Artist { get; set; }
    }
}
