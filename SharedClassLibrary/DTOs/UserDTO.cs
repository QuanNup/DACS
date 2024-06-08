using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.DTOs
{
    public class UserDTO
    {
        public string? Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public bool IsPremium { get; set; }
        public string? ImageUser { get; set; }
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng điền mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng điền nhập lại mật khẩu")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
