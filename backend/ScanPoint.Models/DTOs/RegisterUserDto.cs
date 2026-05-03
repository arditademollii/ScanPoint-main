using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Username është i detyrueshëm.")]
        [MinLength(3, ErrorMessage = "Username duhet të ketë të paktën 3 karaktere.")]
        [MaxLength(50, ErrorMessage = "Username nuk mund të ketë më shumë se 50 karaktere.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email është i detyrueshëm.")]
        [EmailAddress(ErrorMessage = "Email-i nuk është në format të vlefshëm.")]
        [MaxLength(100, ErrorMessage = "Email nuk mund të ketë më shumë se 100 karaktere.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fjalëkalimi është i detyrueshëm.")]
        [MinLength(8, ErrorMessage = "Fjalëkalimi duhet të ketë të paktën 8 karaktere.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Fjalëkalimi duhet të përmbajë të paktën një shkronjë të madhe, një të vogël dhe një numër.")]
        public string Password { get; set; } = string.Empty;

        // ProfileImage është opsionale — Admin mund të regjistrohet pa foto
        public IFormFile? ProfileImage { get; set; }
    }
}