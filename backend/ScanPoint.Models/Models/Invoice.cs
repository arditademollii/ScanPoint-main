using System;
using System.Collections.Generic;

namespace ScanPoint.Models.Models
{
    public class Invoice
    {
        public Guid Id { get; set; }

        public string SerialNumber { get; set; }        // Nr serik
        public DateTime CreatedAt { get; set; }         // Data dhe ora
        public decimal TotalAmount { get; set; }        // Totali

        // ===============================
        // FK → Cashier
        // ===============================
        public Guid CashierId { get; set; }
        public Cashier Cashier { get; set; }

        // ===============================
        // FK → Shop
        // ===============================
        public Guid ShopId { get; set; }
        public Shop Shop { get; set; }

        public List<InvoiceItem> Items { get; set; } = new();
    }
}
