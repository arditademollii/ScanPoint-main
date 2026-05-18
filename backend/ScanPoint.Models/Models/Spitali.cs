using ScanPoint.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.Models
{
    public class Spitali
    {
        [Key]
        public int ID_Spitali { get; set; }          
        public string EmriSpitalit { get; set; } = ""; 
        public int NumriKateve { get; set; } = 0;
        public bool KaUrgjence { get; set; } = false;

        public DateTime DataHapjes { get; set; }


      
        public List<Mjeku> Mjeket { get; set; } = new List<Mjeku>();

        //kena mujt me perdore dhe ICollection<Mjeku> ose IEnumerable<Mjeku>


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
