using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.Entities
{
    public class Artist
    {
        public int ArtistId { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập thông tin tác giả")]
        public string? ArtistName { get; set; }
        public string? ArtistDescription { get; set; }
        public string? Genres { get; set; }
        public string? ArtistImage { get; set; }
        public string? Type { get; set; }
        public string? UserId { get; set; }
        public List<Song>? Songs { get; set; }
        public List<Album>? Albums { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public bool IsApprovedArtist { get; set; }
    }
}
