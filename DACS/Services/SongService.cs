using SharedClassLibrary.Contracts;
using SharedClassLibrary.Entities;

namespace DACS.Services
{
    public class SongService : ISong
    {
        private readonly HttpClient _httpClient;

        public SongService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<List<Song>> SearchSong(string searchText)
        {
            throw new NotImplementedException();
        }
        //public async Task<List<Song>> SearchSong(string searchText)
        //{
        //    return await _httpClient.
        //}
    }
}
