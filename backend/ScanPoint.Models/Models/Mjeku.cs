using ScanPoint.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.Models
{
  
    public class Mjeku
    {
        [Key]
        public int ID { get; set; }              // Çelësi kryesor
        public string EmriMjekut { get; set; } = "";   // Emri
        public decimal Paga { get; set; } = 0;           // Paga

        public DateTime DataPunesimit { get; set; }
        public bool EshteSpecialist { get; set; }


      
        public int ID_Spitali { get; set; }

       
        public Spitali Spitali { get; set; } = null!;
    }
}
