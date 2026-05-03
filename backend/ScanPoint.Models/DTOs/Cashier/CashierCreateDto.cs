using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs.Cashier
{
    public class CashierCreateDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

       
    }
}
