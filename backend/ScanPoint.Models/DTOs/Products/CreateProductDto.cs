using ScanPoint.Models.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs.Products
{
    public class CreateProductDto
    {
        // ✅ Barkodi: minimum 8 shifra (EAN-8), maksimum 14 (EAN-14)
        // Vetëm shifra — pa hapësira, pa shkronja
        [Required(ErrorMessage = "Barkodi është i detyrueshëm.")]
        [RegularExpression(@"^\d{8,14}$",
            ErrorMessage = "Barkodi duhet të ketë 8–14 shifra numerike (standard EAN-8/EAN-13).")]
        public string Barcode { get; set; }

        [Required(ErrorMessage = "Emri është i detyrueshëm.")]
        [MinLength(2, ErrorMessage = "Emri duhet të ketë të paktën 2 karaktere.")]
        public string Name { get; set; }

        // ✅ Çmimi: nuk lejohet 0 as negativ
        [Range(0.01, double.MaxValue, ErrorMessage = "Çmimi duhet të jetë më i madh se 0.")]
        public decimal Price { get; set; }

        // ✅ Njësia: vetëm vlerat e lejuara
        [Required(ErrorMessage = "Njësia është e detyrueshme.")]
        [RegularExpression(@"^(cope|liter|kg|pako)$",
            ErrorMessage = "Njësia duhet të jetë: cope, liter, kg ose pako.")]
        public string Unit { get; set; }

        [Required(ErrorMessage = "Data e skadimit është e detyrueshme.")]
        public DateTime ExpiryDate { get; set; }

        public ProductCategory Category { get; set; }

        // ✅ Sasia: nuk lejohet 0 as negative
        [Range(1, int.MaxValue, ErrorMessage = "Sasia duhet të jetë të paktën 1.")]
        public int Quantity { get; set; }

        public Guid? ShopId { get; set; }
    }
}