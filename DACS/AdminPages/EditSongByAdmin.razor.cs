using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SharedClassLibrary.Entities;
using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace DACS.AdminPages
{
    public partial class EditSongByAdmin
    {
        [Parameter]
        public int? Id { get; set; }
        public Song Song = new Song();
        public List<SongGenre> songGenres = new List<SongGenre>();
        public List<Artist> artists = new List<Artist>();
        private IBrowserFile? imageFile;
        private IBrowserFile? songFile;
        private string Msg = "Đã đăng thành công!";
        private string MsgErr = "Đã đăng thành công!";

        protected override async Task OnInitializedAsync()
        {

            if (Id is not null)
            {
                var result = await Http.GetFromJsonAsync<Song>($"api/songs/{Id}");
                if (result is not null)
                {
                    Song = result;
                }
            }
            var songgenres = await Http.GetFromJsonAsync<List<SongGenre>>("api/song-genres");
            if (songgenres is not null)
            {
                songGenres = songgenres;
            }
            var response = await Http.GetFromJsonAsync<List<Artist>>("api/artists");
            if (response != null)
            {
                artists = response;
            }
        }
        private async Task HandleSubmit()
        {
            var maxAllowedSize = 30 * 1024 * 1024;
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(Song.SongName ?? string.Empty), "SongName");
            content.Add(new StringContent(Song.ArtistId.ToString()), "ArtistId");
            content.Add(new StringContent(Song.SongDescription ?? string.Empty), "SongDescription");
            content.Add(new StringContent(Song.SongGenreId.ToString()), "SongGenreId");
            if (imageFile != null)
            {
                var imageStreamContent = new StreamContent(imageFile.OpenReadStream(maxAllowedSize));
                imageStreamContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
                content.Add(imageStreamContent, "imageFile", imageFile.Name);
            }
            if (songFile != null)
            {
                var musicStreamContent = new StreamContent(songFile.OpenReadStream(maxAllowedSize));
                musicStreamContent.Headers.ContentType = new MediaTypeHeaderValue(songFile.ContentType);
                content.Add(musicStreamContent, "songFile", songFile.Name);
            }
            HttpResponseMessage result;
            if (Id is not null)
            {
                result = await Http.PutAsync($"api/songs/{Id}", content);
            }
            else
            {
                result = await Http.PostAsync("api/songs", content);
            }

            if (result.IsSuccessStatusCode)
            {

                Song = await result.Content.ReadFromJsonAsync<Song>();
                await JSRuntime.InvokeVoidAsync("showToast",Msg);
                NavigationManager.NavigateTo("/");
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("showToast", MsgErr);
            }

        }



        private async Task HandleSelectedFile(InputFileChangeEventArgs e)
        {
            imageFile = e.File;

            if (imageFile != null)
            {
                await JSRuntime.InvokeVoidAsync("previewImage", "upload_img", "image-preview");
                SongImgfileName = imageFile.Name;
            }

        }
        private string SongImgfileName;
        private string SongfileName;

        private void UpdateFileName(InputFileChangeEventArgs e)
        {
            songFile = e.File;

            if (songFile != null)
            {
                SongfileName = songFile.Name;
            }
        }
    }
}
