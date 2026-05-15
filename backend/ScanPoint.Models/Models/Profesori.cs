using ScanPoint.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.Models
{
    // Tabela e nxënësit — lidhur me shkollën
    public class Profesori
    {
        [Key]
        public int ID { get; set; }              // Çelësi kryesor
        public string EmriProfesorit { get; set; } = "";   // Emri
        public string Lenda { get; set; } = "";           // Lenda që jep profesori

        // Foreign key— e lidh nxënesin me shkollën e tij
        // Kjo krijon relacion "SHUMË-me-NJË": shumë nxënës → një shkollë
        public int ID_Universiteti { get; set; }

        // Navigation property — universiteti i profesorëve, pra qe te mundeemi ta therrasim si profesori.Universiteti.EmriUniversitetit
        public Universiteti Universiteti { get; set; } = null!;
    }
}
