using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Forms;
using SharedClassLibrary.Entities;

namespace DACS.Services
{
    public class AdminServicecs /*: ISong*/
    {
        public AdminServicecs(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            this.httpClient = httpClient;
            this.localStorageService = localStorageService;
        }
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;
        public Song Song = new Song();
        public List<SongGenre> songGenres = new List<SongGenre>();
        private IBrowserFile? imageFile;
        private IBrowserFile? songFile;
        //public async Task<Song> PostSongByAdmin()
        //{
        //    var maxAllowedSize = 30 * 1024 * 1024;
        //    var content = new MultipartFormDataContent();
        //    content.Add(new StringContent(Song.SongName ?? string.Empty), "SongName");
        //    content.Add(new StringContent(Song.ArtistId.ToString()), "ArtistId");
        //    content.Add(new StringContent(Song.SongDescription ?? string.Empty), "SongDescription");
        //    content.Add(new StringContent(Song.SongGenreId.ToString()), "SongGenreId");
        //    if (imageFile != null)
        //    {
        //        var imageStreamContent = new StreamContent(imageFile.OpenReadStream(maxAllowedSize));
        //        imageStreamContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
        //        content.Add(imageStreamContent, "imageFile", imageFile.Name);
        //    }
        //    if (songFile != null)
        //    {
        //        var musicStreamContent = new StreamContent(songFile.OpenReadStream(maxAllowedSize));
        //        musicStreamContent.Headers.ContentType = new MediaTypeHeaderValue(songFile.ContentType);
        //        content.Add(musicStreamContent, "songFile", songFile.Name);
        //    }
        //    var response = await httpClient.PostAsync("api/songs/AddSongByAdmin", content);
        //    return response;
        //}
    }
}
