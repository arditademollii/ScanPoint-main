using System;
using System.Collections.Generic;

namespace ScanPoint.Models.Models
{
    public enum ProductCategory
    {
        Pije,
        Ushqim,
        Higjien,
        Tjera
    }

    public class Product
    {
        public Guid Id { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public DateTime ExpiryDate { get; set; }
        public ProductCategory Category { get; set; }
        public int Quantity { get; set; }

        // 🔹 Soft delete flag
        public bool IsDeleted { get; set; } = false;

        // 🔑 FK → Shop
        public Guid ShopId { get; set; }
        public Shop Shop { get; set; }

        public List<InvoiceItem> InvoiceItems { get; set; } = new();
       
    }
}
