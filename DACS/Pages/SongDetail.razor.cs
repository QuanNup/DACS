using DACS.AdminPages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SharedClassLibrary.Contracts;
using SharedClassLibrary.Entities;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;

namespace DACS.Pages
{
    public partial class SongDetail
    {
        [Parameter]
        public int? Id { get; set; }
        public Song? Song = new Song();
        public Artist? Artist = new Artist();
        public LikedSong? LikedSong = new LikedSong();
        private string? imageUrl;
        private double totalTime = 0;
        private bool isPlaying = false;
        private string totalTimeString => TimeSpan.FromSeconds(totalTime).ToString(@"mm\:ss");
        private string? songUrl;
        private bool isLiked;
        private Dictionary<string, string> formattedSongTimes = new Dictionary<string, string>();
        private Dictionary<int, bool> isLikedSong = new Dictionary<int, bool>();
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var result = await Http.GetFromJsonAsync<Song>($"api/songs/{Id}");
            if (result is not null)
            {
                Song = result;
            }
            var artistsong = await Http.GetFromJsonAsync<Artist>($"api/artists/artistwithsong/{Song?.ArtistId}");
            if (artistsong is not null)
            {
                Artist = artistsong;
            }

            if (Song != null)
            {
                if(Song.SongImage != null)
                {
                    var response = await Http.GetAsync($"api/songs/file-images?imageName={Song.SongImage}");
                    if (response.IsSuccessStatusCode)
                    {
                        var imageBytes = await response.Content.ReadAsByteArrayAsync();
                        imageUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";
                    }
                }
                else
                {
                    imageUrl = "/images/Spotify_favicon.png";
                }
                songUrl = $"https://localhost:7196/api/songs/file?filePath={Uri.EscapeDataString(Song.SongFile)}";
                StateHasChanged();
            }
            if (!string.IsNullOrEmpty(songUrl))
            {
                totalTime = await JSRuntime.InvokeAsync<double>("audioHelper.getAudioDuration", songUrl);
            }
            await LoadSongDataAsync();
           
        }
        protected override async Task OnParametersSetAsync()
        {
            var result = await Http.GetFromJsonAsync<Song>($"api/songs/{Id}");
            if (result is not null)
            {
                Song = result;
                StateHasChanged();
            }
            var artistsong = await Http.GetFromJsonAsync<Artist>($"api/artists/artistwithsong/{Song?.ArtistId}");
            if (artistsong is not null)
            {
                Artist = artistsong;
                StateHasChanged();
            }
            await LoadSongDataAsync();
            isLiked = await IsLikedSong((int)Id);
            StateHasChanged();
        }

        private async Task LoadSongDataAsync()
        {
            if (Artist != null && Artist.Songs != null && formattedSongTimes != null)
            {
                var tasks = Artist.Songs.Select(async song =>
                {
                    var formattedTime = await GetFormattedTime(song.SongFile);
                    var isLiked = await IsLikedSong(song.SongId);
                    if (formattedTime != null)
                    {
                        formattedSongTimes[song.SongFile] = formattedTime;
                    }
                    isLikedSong[song.SongId] = isLiked;
                });
                await Task.WhenAll(tasks);
                StateHasChanged(); 
            }
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

        private async Task<double> GetTotalTime(string songUrl)
        {
            return await JSRuntime.InvokeAsync<double>("audioHelper.getAudioDuration", songUrl);

        }

        private async Task<string> GetFormattedTime(string songName)
        {
            var songUrl = GetSongUrl(songName);
            var totalTime = await GetTotalTime(songUrl);
            return TimeSpan.FromSeconds(totalTime).ToString(@"mm\:ss");
        }

        private async Task PlayPause()
        {
            if (!isPlaying)
            {
                songData.Song = Song;
                StateHasChanged();
                await Task.Delay(100);
                await JSRuntime.InvokeVoidAsync("audioHelper.playAudio");
                isPlaying = true;
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("audioHelper.pauseAudio");
                isPlaying = false;
            }
        }

        void Details(int id)
        {
            NavigationManager.NavigateTo($"Song-Detail/{id}");
        }
        private async Task<bool> IsLikedSong(int id)
        {
            try
            {
                var response = await Http.GetFromJsonAsync<bool>($"/api/likedsongs/{id}");
                return response;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking like status: {ex.Message}");
                return false;
            }
            
        }
        private async Task ToggleLike(int id)
        {
            try
            {
                if (isLiked)
                {
                    var response = await Http.DeleteAsync($"/api/likedsongs/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Error unliking song.");
                    }
                }
                else
                {
                    var response = await Http.PostAsJsonAsync($"/api/likedsongs/{id}", LikedSong);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Error liking song.");
                    }
                }
                isLiked = !isLiked;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling like: {ex.Message}");
            }
        }
        private async Task ToggleLiked(int songId)
        {
            try
            {
                if (isLikedSong[songId])
                {
                    var response = await Http.DeleteAsync($"/api/likedsongs/{songId}");
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Error unliking song.");
                    }
                }
                else
                {
                    var response = await Http.PostAsJsonAsync($"/api/likedsongs/{songId}", LikedSong);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Error liking song.");
                    }
                }

                isLikedSong[songId] = !isLikedSong[songId];
                StateHasChanged(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling like: {ex.Message}");
            }
        }
    }
}
