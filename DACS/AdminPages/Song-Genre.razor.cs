
using Microsoft.AspNetCore.Components;
using SharedClassLibrary.Entities;
using System.Net.Http.Json;

namespace DACS.AdminPages
{
    public partial class Song_Genre
    {
        [Parameter]
        public int? Id { get; set; }
        private SongGenre? songGenres = new SongGenre();
        protected override async Task OnInitializedAsync()
        {
            if (Id is not null)
            {
                songGenres = await Http.GetFromJsonAsync<SongGenre>($"api/song-genres/{Id}");
            }
        }
        private async Task HandleSubmit()
        {
            HttpResponseMessage result;
            if(Id is not null)
            {
                result = await Http.PutAsJsonAsync($"api/song-genres/{Id}",songGenres);
            }
            else
            {
                result = await Http.PostAsJsonAsync("api/song-genres",songGenres);
            }
            if (result.IsSuccessStatusCode)
            {
                songGenres = await result.Content.ReadFromJsonAsync<SongGenre>();
                NavigationManager.NavigateTo("/");
            }
            else
            {
                var errorMessage = await result.Content.ReadAsStringAsync();
            }
        }
    }
}
