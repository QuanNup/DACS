﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SharedClassLibrary.Entities;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DACS.AdminPages
{
    public partial class AddArtists
    {

        [Parameter]
        public int? Id { get; set; }
        public Artist artists = new Artist();
        private IBrowserFile? imageFile;
        protected override async Task OnInitializedAsync()
        {
            if (Id is not null)
            {
                var result = await Http.GetFromJsonAsync<Artist>($"api/artists/{Id}");
                if (result is not null)
                {
                    artists = result;
                }
            }
        }
        private async Task HandleSubmit()
        {
            var maxAllowedSize = 30 * 1024 * 1024;
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(artists.ArtistName ?? string.Empty), "ArtistName");
            content.Add(new StringContent(artists.Genres ?? string.Empty), "Genres");
            content.Add(new StringContent(artists.Type ?? string.Empty), "Type");
            content.Add(new StringContent(artists.ArtistDescription ?? string.Empty), "ArtistDescription");
            content.Add(new StringContent(artists.UserId ?? string.Empty), "UserId");

            if (imageFile != null)
            {
                var imageStreamContent = new StreamContent(imageFile.OpenReadStream(maxAllowedSize));
                imageStreamContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
                content.Add(imageStreamContent, "imageFile", imageFile.Name);
            }
            HttpResponseMessage result;
            if (Id is not null)
            {
                result = await Http.PutAsync($"api/artists/{Id}", content);
            }
            else
            {
                result = await Http.PostAsync("api/artists", content);
            }

            if (result.IsSuccessStatusCode)
            {

                artists = await result.Content.ReadFromJsonAsync<Artist>();
                //await JSRuntime.InvokeVoidAsync("showToast");
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

            }

        }
    }
}
