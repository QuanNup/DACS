
using DACS.DataShared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SharedClassLibrary.Contracts;
using SharedClassLibrary.Entities;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace DACS.Pages
{
    public partial class LikedSongs
    {
        private LikedSong? likedSongs = new LikedSong();
        //private string? UserId;
        private int index;
        private int totalSong;
        private bool isLiked = true;
        private bool isPlaying=false;
        private Dictionary<string, string> formattedSongTimes = new Dictionary<string, string>();
        private Dictionary<int, bool> isPlayingSongs = new Dictionary<int, bool>();
        private int currentSongIndex = 0;
        private bool isAnySongPlaying = false;
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                //var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                //var user = authState.User;
                //var UserIdClaims = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                //if (UserIdClaims != null)
                //{
                //    UserId = UserIdClaims.Value;
                //}
                var response = await Http.GetFromJsonAsync<LikedSong>($"api/likedsongs/GetLikedSongs");
                if (response == null)
                {
                    likedSongs = null;
                }
                else
                {                    
                    likedSongs = response;
                    totalSong = likedSongs.LikedSongDetails.Count;
                }
                await LoadSongDataAsync();
            }
            catch (HttpRequestException)
            {
                likedSongs = null;
            }
            catch (JsonException)
            {
                likedSongs = null; 
            }
        }

        private async Task LoadSongDataAsync()
        {
            if (likedSongs != null && likedSongs.LikedSongDetails != null && formattedSongTimes != null)
            {
                var tasks = likedSongs.LikedSongDetails.Select(async song =>
                {
                    var formattedTime = await GetFormattedTime(song.Song.SongFile);
                    if (formattedTime != null)
                    {
                        formattedSongTimes[song.Song.SongFile] = formattedTime;
                    }
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

        private async Task ToggleLike(int id)
        {
            try
            {
                
                var response = await Http.DeleteAsync($"/api/likedsongs/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error unliking song.");
                }
                else
                {
                    likedSongs = await Http.GetFromJsonAsync<LikedSong>($"api/likedsongs/GetLikedSongs");
                    totalSong = likedSongs.LikedSongDetails.Count;
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling like: {ex.Message}");
            }
        }
        private async Task PlayPause(int songId = -1) 
        {
            if (likedSongs == null || likedSongs.LikedSongDetails.Count == 0)
                return;
            if (songId != -1)
            {
                var currentPlayingSong = isPlayingSongs.FirstOrDefault(x => x.Value);
                if (currentPlayingSong.Key != 0 && currentPlayingSong.Key != songId) 
                {
                    isPlayingSongs[currentPlayingSong.Key] = false;
                    await JSRuntime.InvokeVoidAsync("audioHelper.pauseAudio");
                }

                if (!isPlayingSongs.ContainsKey(songId))
                {
                    isPlayingSongs[songId] = true;
                }
                else
                {
                    isPlayingSongs[songId] = !isPlayingSongs[songId];
                }

                if (isPlayingSongs[songId])
                {
                    await PlaySong(songId);
                    await Task.Delay(100);
                    await JSRuntime.InvokeVoidAsync("audioHelper.playAudio");
                    currentSongIndex = likedSongs.LikedSongDetails.FindIndex(s => s.SongId == songId); 
                    isPlaying = true;
                    isAnySongPlaying = true;
                    
                }
                else
                {
                    isPlaying = false;
                    isAnySongPlaying = false;
                    await JSRuntime.InvokeVoidAsync("audioHelper.pauseAudio");
                }
            }
            else
            {
                if (!isAnySongPlaying)
                {
                    await JSRuntime.InvokeVoidAsync("audioHelper.playAudio");
                    isAnySongPlaying = true;
                    isPlaying = true;
                    isPlayingSongs[songId] = true;
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("audioHelper.pauseAudio");
                    isPlaying = false;
                    isAnySongPlaying = false;
                    isPlayingSongs[songId] = false;
                }
                StateHasChanged();
            }
            StateHasChanged();
        }


        private async Task PlaySong(int songId)
        {
            songData.Song = await Http.GetFromJsonAsync<Song>($"api/songs/{songId}");
        }

    }
}
