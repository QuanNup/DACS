

using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.Entities
{
    public class Song
    {
        public int SongId { get; set; }
        public string? SongName { get; set; }
        public string? SongDescription { get; set; }
        public string? SongImage { get; set; }
        public string? SongFile { get; set; }
        public bool? IsApproved { get; set; }
        public int SongGenreId { get; set; }
        public virtual SongGenre? SongGenre { get; set; }
        public int ArtistId { get; set; }
        public virtual Artist? Artist { get; set; }
        public int? AlbumId { get; set; }
        public virtual Album? Album { get; set; }
    }
}
