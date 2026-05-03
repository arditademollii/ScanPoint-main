using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.Shop
{
    public class CreateShopDto
    {
        public string Name { get; set; }

        [Required]
        [RegularExpression(@".*[a-zA-ZÀ-ÿ].*",
       ErrorMessage = "Adresa duhet të përmbajë të paktën një shkronjë.")]
        public string Address { get; set; }
        public string FiscalNumber { get; set; }
        public string VatNumber { get; set; }
    }
}
