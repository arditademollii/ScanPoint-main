using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.Shop
{
    public class ShopDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string VatNumber { get; set; }
        public string FiscalNumber { get; set; }
        public string AdminName { get; set; }
    }
}
