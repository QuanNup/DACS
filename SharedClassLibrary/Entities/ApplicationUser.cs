using Microsoft.AspNetCore.Identity;

namespace SharedClassLibrary.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public bool IsPremium { get; set; }
        public string? ImageUser {  get; set; }
    }
}
