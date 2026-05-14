using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.Models
{
    public class Punetori
    {
        [Key]
        public int ID { get; set; }              // Çelësi kryesor
        public string EmriPunetorit { get; set; } = "";   // Emri
        public string MbiemriPunetorit { get; set; } = "";           // Klasa (p.sh. "10A", "11B")

        public string Pozita { get; set; } = "";
        // Foreign key— e lidh nxënesin me shkollën e tij
        // Kjo krijon relacion "SHUMË-me-NJË": shumë nxënës → një shkollë
        public int ID_Fabrika { get; set; }

        // Navigation property — shkolla e nxënësit, pra qe te mundeemi ta therrasim si nxenesi.Shkolla.EmriShkolles
        public Fabrika Fabrika { get; set; } = null!;
    }
}
