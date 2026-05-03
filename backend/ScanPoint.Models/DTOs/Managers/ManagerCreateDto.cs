using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.Managers
{
    public class ManagerCreateDto
    {
        [Required(ErrorMessage = "Username është i detyrueshëm.")]
        [MinLength(3)]
        public string Username { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = "";

        [Required]
        public Guid? ShopId { get; set; }
    }
}
