
namespace SharedClassLibrary.Entities
{
    public class LikedSongDetails
    {
        public int Id { get; set; }
        public int LikedSongId { get; set; }
        public int SongId { get; set; }
        public Song? Song { get; set; }
        public required LikedSong LikedSong { get; set; }
    }
}
