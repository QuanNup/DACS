using SharedClassLibrary.Entities;
namespace SharedClassLibrary.Contracts
{
    public interface IArtist
    {
        Task<Artist> GetArtistWithUserId();
        //Task<Artist> GetArtist();
    }
}
