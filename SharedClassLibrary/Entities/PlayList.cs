

namespace SharedClassLibrary.Entities
{
    public class PlayList
    {
        public int PlayListId { get; set; }
        public string? PlayListName { get; set; }
        public string? UserId { get; set; }
        public DateOnly? PlayListDay { get; set; }
        public ApplicationUser? User { get; set; }
        public List<PlayListDetails>? PlayListDetails { get; set; }
    }
}
