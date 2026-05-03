using ScanPoint.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.Products
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public DateTime ExpiryDate { get; set; }
        public ProductCategory Category { get; set; }
        public int Quantity { get; set; }
        public Guid ShopId { get; set; } // e vendos useri (admin) ose merret nga manager
    }
}
