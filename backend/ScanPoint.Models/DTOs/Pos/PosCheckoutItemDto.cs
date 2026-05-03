using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.Pos
{
    public class PosCheckoutItemDto
    {
        [Required]
        public string Barcode { get; set; }

        [Range(1, 1000)]
        public int Quantity { get; set; }
    }
}
