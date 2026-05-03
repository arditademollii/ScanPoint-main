using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.Pos
{
    public class InvoiceDetailsDto
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }

        public CashierBasicDto? Cashier { get; set; }
        public ShopBasicDto? Shop { get; set; }

        public List<InvoiceItemReadDto> Items { get; set; }
    }

}
