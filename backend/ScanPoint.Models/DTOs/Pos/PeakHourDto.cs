using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.Pos
{
    public class PeakHourDto
    {
        public int Hour { get; set; } // 0–23
        public int TotalInvoices { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
