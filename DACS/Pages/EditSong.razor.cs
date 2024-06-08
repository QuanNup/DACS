
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SharedClassLibrary.DTOs;
using SharedClassLibrary.Entities;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;


namespace DACS.Pages
{
    public partial class EditSong
    {
        private bool isArtist = false;
        private string Msg = "Đã gửi yêu cầu thành công!";
        private string msgNotArtist = "Bạn chưa đăng ký nghệ sĩ!";
        [Parameter]
        public int? Id { get; set; }
        private Artist? artist = new Artist();
        private Song Song = new Song();
        private List<SongGenre> songGenres = new List<SongGenre>();
        private IBrowserFile? imageFile;
        private IBrowserFile? songFile;

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
            
            try
            {
                artist = await ArtistService.GetArtistWithUserId();
                
            }
            catch (HttpRequestException)
            {
                artist = null;
            }
            catch (JsonException)
            {
                artist = null;
            }
            if (artist is not null)
            {
                isArtist = true;
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("showBox", msgNotArtist);
            }


        }
        private async Task HandleSubmit()
        {
            var maxAllowedSize = 30 * 1024 * 1024;
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(Song.SongName ?? string.Empty), "SongName");
            content.Add(new StringContent(artist.ArtistId.ToString()), "ArtistId");
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
                //await JSRuntime.InvokeVoidAsync("showToast",Msg);
                //await Task.Delay(3000);
                NavigationManager.NavigateTo("/");
            }
            else
            {
                var errorMessage = await result.Content.ReadAsStringAsync();
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
