using Microsoft.JSInterop;
using SharedClassLibrary.Contracts;
using SharedClassLibrary.Entities;
using System.Net.Http.Json;

namespace DACS.Pages
{
    public partial class Home
    {
        private Random random = new Random();
        List<Song> Songs = new List<Song>();

        private IEnumerable<Song> GetRandomSongs(List<Song> songs, int count)
        {
            return songs.OrderBy(x => random.Next()).Take(count);
        }
        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(500);
            var result = await Http.GetFromJsonAsync<List<Song>>("api/songs/IsApproved");
            if (result is not null)
            {
                Songs = result;

            }
            
        }
        protected override async Task OnParametersSetAsync()
        {
           
        }

        void SongDetail(int id)
        {
            NavigationManager.NavigateTo($"Song-Detail/{id}");
        }
        
        
    }
}
