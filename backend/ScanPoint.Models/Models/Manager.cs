using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.Models
{
    public class Manager : User
    {
        
        public List<Cashier> ManagedCashiers { get; set; } = new List<Cashier>();

        
    }

}
