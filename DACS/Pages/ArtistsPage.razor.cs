using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedClassLibrary.Entities;
using System.Net.Http.Json;

namespace DACS.Pages
{
    public partial class ArtistsPage
    {
        [Parameter]
        public int? Id { get; set; }
        private Artist artist = new Artist();
        private string imageUrl;
        private string? songUrl;
        private string totalTimeString => TimeSpan.FromSeconds(totalTime).ToString(@"mm\:ss");
        private double totalTime = 0;
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var response = await http.GetFromJsonAsync<Artist>($"api/artists/{Id}");
            if (response != null)
            {
                artist = response;
            }
            if (artist != null)
            {
                var selectedSong = artist.Songs.FirstOrDefault();
                imageUrl = $"https://localhost:7196/api/songs/file-images?imageName={Uri.EscapeDataString(selectedSong.SongImage)}";
                songUrl = $"https://localhost:7196/api/songs/file?filePath={Uri.EscapeDataString(selectedSong.SongFile)}";
                StateHasChanged();
            }
            if (!string.IsNullOrEmpty(songUrl))
            {
                totalTime = await JSRuntime.InvokeAsync<double>("audioHelper.getAudioDuration", songUrl);
            }
            Console.WriteLine(artist.ArtistName);
        }
        private string GetImageUrl(string? imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                string baseUrl = $"https://localhost:7196/api/songs/file-images?imageName=";
                return $"{baseUrl}{Uri.EscapeDataString(imageName)}";
            }
            return "/images/Spotify_favicon.png";
        }
        private string GetSongUrl(string? songName)
        {
            string baseUrl = $"https://localhost:7196/api/songs/file?filePath=";
            return $"{baseUrl}{Uri.EscapeDataString(songName)}";
        }
    }
}
