using ScanPoint.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.Models
{
  
    public class Nxenesi
    {
        [Key]
        public int ID { get; set; }             
        public string EmriNxenesit { get; set; } = "";   
        public string Klasa { get; set; } = "";         

        
        public int ID_Shkolla { get; set; }

        
        public Shkolla Shkolla { get; set; } = null!;
    }
}
