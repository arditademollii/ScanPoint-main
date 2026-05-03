using System;
using System.Collections.Generic;

namespace ScanPoint.Models.Models
{
    public class Shop
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string FiscalNumber { get; set; }
        public string VatNumber { get; set; }

        // ✅ Soft Delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Admin / pronar i dyqanit
        public Guid AdminId { get; set; }
        public User Admin { get; set; }

        // Punonjësit
        public List<Cashier> Cashiers { get; set; } = new List<Cashier>();
        public List<Manager> Managers { get; set; } = new List<Manager>();

        // Produkte dhe Invoices
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}