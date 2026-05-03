using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.Cashier
{
    public class CashierUpdateDto
    {
        [Required(ErrorMessage = "Username është i detyrueshëm")]
        [MinLength(3, ErrorMessage = "Username duhet të ketë së paku 3 karaktere")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email është i detyrueshëm")]
        [EmailAddress(ErrorMessage = "Format i pavlefshëm i email-it")]
        public string Email { get; set; }

        // Password-i nuk është i detyrueshëm gjatë Update (vetëm nëse duam ta ndryshojmë)
        public string? Password { get; set; }

        
    }
}
