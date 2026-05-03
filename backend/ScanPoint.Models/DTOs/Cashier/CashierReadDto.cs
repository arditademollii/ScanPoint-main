using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs.Cashier
{
    public class CashierReadDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
       
        public string? ManagerUsername { get; set; }
        public string? ShopName { get; set; }
    }
}
