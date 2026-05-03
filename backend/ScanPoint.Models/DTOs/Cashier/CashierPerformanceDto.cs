using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.Cashier
{
    public class CashierPerformanceDto
    {
        public Guid CashierId { get; set; }
        public string CashierName { get; set; }

        public int TotalInvoices { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AveragePerInvoice { get; set; }
        public int TotalItemsSold { get; set; }
    }
}
