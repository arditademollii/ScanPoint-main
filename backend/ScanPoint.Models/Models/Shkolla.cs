using ScanPoint.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.Models
{
    // Tabela e shkollës — entity kryesore
    public class Shkolla
    {
        [Key]
        public int ID_Shkolla { get; set; }          // Çelësi kryesor
        public string EmriShkolles { get; set; } = "";  // Emri i shkollës
        public string Qyteti { get; set; } = "";        // Qyteti ku ndodhet

        // Navigation property — lista e nxënësve që i përkasin kësaj shkolle
        // "Një shkollë ka SHUMË nxënës"
        public List<Nxenesi> Nxenesit { get; set; } = new List<Nxenesi>();
    }
}
