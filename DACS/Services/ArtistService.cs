using Blazored.LocalStorage;
using SharedClassLibrary.Contracts;
using SharedClassLibrary.Entities;
using System.Net.Http.Json;

namespace DACS.Services
{
    public class ArtistService : IArtist
    {
        public ArtistService(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            this.httpClient = httpClient;
            this.localStorageService = localStorageService;
        }

        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;

        public async Task<Artist> GetArtistWithUserId()
        {
            var response = await httpClient.GetFromJsonAsync<Artist>("api/Artists/current");
            if (response != null)
            {
                return response;
            }
            return null;
        }
    }
}
