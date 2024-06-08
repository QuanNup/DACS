using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.Entities;


namespace DACSServerApi.Data
{
    public class AppDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Song> Songs { get; set; }
        public DbSet<SongGenre> SongsGenres { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<PlayList> Playlists { get; set; }
        public DbSet<LikedSong> LikedSongs { get; set; }
        public DbSet<LikedSongDetails> LikedSongsDetails { get; set; }
        public DbSet<PlayListDetails> PlaylistsDetails { get; set; }
    }
}
