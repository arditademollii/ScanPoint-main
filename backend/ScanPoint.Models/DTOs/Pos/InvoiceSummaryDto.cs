using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.Pos
{
    public class InvoiceSummaryDto
    {
        public DateTime Date { get; set; }               // Data e transaksioneve
        public string ShopName { get; set; }            // Emri i dyqanit
        public string CashierName { get; set; }         // Emri i cashier-it
        public decimal TotalAmount { get; set; }        // Totali i të gjitha invoices
        public int InvoiceCount { get; set; }           // Numri i invoices për këtë grup
    }
}
