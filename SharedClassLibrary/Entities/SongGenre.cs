
using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.Entities
{
    public class SongGenre
    {
        public int SongGenreId { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập nhập tên thể loại!")]
        public string? SongGenreName { get; set; }
    }
}
