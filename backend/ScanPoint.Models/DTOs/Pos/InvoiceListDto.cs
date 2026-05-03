using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.Pos
{
    public class InvoiceListDto
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }

        // 🔹 për frontend
        public string ShopName { get; set; }
        public string CashierName { get; set; }
    }



}
