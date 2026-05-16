using ScanPoint.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.Models
{
    // Tabela e nxënësit — lidhur me shkollën
    public class Mjeku
    {
        [Key]
        public int ID { get; set; }              // Çelësi kryesor
        public string EmriMjekut { get; set; } = "";   // Emri
        public decimal Paga { get; set; } = 0;           // Paga

        public DateTime DataPunesimit { get; set; }
        public bool EshteSpecialist { get; set; }


        // Foreign key— e lidh nxënesin me shkollën e tij
        // Kjo krijon relacion "SHUMË-me-NJË": shumë nxënës → një shkollë
        public int ID_Spitali { get; set; }

        // Navigation property — shkolla e nxënësit, pra qe te mundeemi ta therrasim si nxenesi.Shkolla.EmriShkolles
        public Spitali Spitali { get; set; } = null!;
    }
}
