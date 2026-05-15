using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.Models
{
    public class Ligjerata
    {
        [Key]
        public int ID { get; set; }              // Çelësi kryesor
        public string EmriLigjerates { get; set; } = "";   // Emri
             

        // Foreign key— e lidh nxënesin me shkollën e tij
        // Kjo krijon relacion "SHUMË-me-NJË": shumë nxënës → një shkollë
        public int ID_Ligjeruesi { get; set; }

        // Navigation property — shkolla e nxënësit, pra qe te mundeemi ta therrasim si nxenesi.Shkolla.EmriShkolles
        public Ligjeruesi Ligjeruesi { get; set; } = null!;
    
}
}
