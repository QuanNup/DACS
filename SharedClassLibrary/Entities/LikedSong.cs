namespace SharedClassLibrary.Entities
{
    public class LikedSong
    {
        public int LikedSongId { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public List<LikedSongDetails> LikedSongDetails { get; set; }
    }
}
