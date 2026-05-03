using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.Pos
{
    public class PosCheckoutRequestDto
    {
        public List<PosCheckoutItemDto> Items { get; set; }
        public decimal AmountPaid { get; set; }
    }
}
