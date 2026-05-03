using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.Pos
{
    public class PosInvoiceDto
    {
        public Guid InvoiceId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<PosInvoiceItemDto> Items { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Change { get; set; }
    }
}
