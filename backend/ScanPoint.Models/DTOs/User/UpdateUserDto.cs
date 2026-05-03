using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.User
{
    public class UpdateUserDto
    {
        public string? Username { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [MinLength(6)]
        public string? Password { get; set; }

        public IFormFile? File { get; set; }
    }
}
