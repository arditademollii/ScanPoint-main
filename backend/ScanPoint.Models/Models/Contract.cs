using ScanPoint.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.Models
{
    // Tabela e nxënësit — lidhur me shkollën
    public class Contract
    {
        [Key]
        public int ID { get; set; }              // Çelësi kryesor
        public string Title { get; set; } = "";   // Emri
        public string Description { get; set; } = "";           // Klasa (p.sh. "10A", "11B")

        // Foreign key— e lidh nxënesin me shkollën e tij
        // Kjo krijon relacion "SHUMË-me-NJË": shumë nxënës → një shkollë
        public int ID_Employee { get; set; }

        // Navigation property — shkolla e nxënësit, pra qe te mundeemi ta therrasim si nxenesi.Shkolla.EmriShkolles
        public Employee Employee { get; set; } = null!;
    }
}
