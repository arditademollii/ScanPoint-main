using ScanPoint.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.Models
{
   
    public class Shkolla
    {
        [Key]
        public int ID_Shkolla { get; set; }         
        public string EmriShkolles { get; set; } = "";  
        public string Qyteti { get; set; } = "";      

       
        public List<Nxenesi> Nxenesit { get; set; } = new List<Nxenesi>();

        //kena mujt me perdore dhe ICollection<Nxenesi> ose IEnumerable<Nxenesi>




        /* 1me1
        Navigation property
        public Pasaporta Pasaporta { get; set; } = null!;
            dhe 
        // Navigation property
    public Personi Personi { get; set; } = null!;


        NmeN
         // Shumë lëndë
    public List<Lenda> Lendet { get; set; } = new();
           dhe
    // Shumë studentë
    public List<Contract> Contract { get; set; } = new();



        */
    }
}
