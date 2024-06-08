

namespace SharedClassLibrary.Entities
{
    public class PlayListDetails
    {
        public int Id { get; set; }
        public int PlayListId { get; set; }
        public required PlayList PlayList { get; set; }
        public List<Song>? Song { get; set; }
    }
}
