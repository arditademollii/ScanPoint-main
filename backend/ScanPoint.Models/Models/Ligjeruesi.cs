using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.Models
{
    public class Ligjeruesi
    {
        [Key]
        public int ID_Ligjeruesi { get; set; }          // Çelësi kryesor
        public string EmriLigjeruesit { get; set; } = "";  // Emri i shkollës
        public string Departamenti { get; set; } = "";        // Qyteti ku ndodhet

        public string Email { get; set; } = "";
        // Navigation property — lista e nxënësve që i përkasin kësaj shkolle
        // "Një shkollë ka SHUMË nxënës"
        public List<Ligjerata> Ligjeratat { get; set; } = new List<Ligjerata>();
    }
}
