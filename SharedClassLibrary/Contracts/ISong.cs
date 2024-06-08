using SharedClassLibrary.Entities;

namespace SharedClassLibrary.Contracts
{
    public interface ISong
    {
        Task<List<Song>> SearchSong(string searchText);
    }
}
