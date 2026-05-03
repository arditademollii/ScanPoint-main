using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.Models
{
    public class InvoiceItem
    {
        public Guid Id { get; set; }

        // FK për Product
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        // FK për Invoice
        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public string Name { get; set; }        // Emri i produktit
        public decimal Price { get; set; }      // Çmimi i produktit
        public int Quantity { get; set; }       // Sasia e blerë

        public decimal SubTotal => Price * Quantity; // Totali për produkt
    }


}
