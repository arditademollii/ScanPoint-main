using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.Models
{
    public class Cashier : User
    {
       
        public Guid ManagerId { get; set; }
        public Manager Manager { get; set; }
        public List<Invoice> Invoices { get; set; } = new List<Invoice>(); // 1:N
    }

}
