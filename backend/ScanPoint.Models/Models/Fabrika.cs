using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.Models
{
    public class Fabrika
    {
        [Key]
        public int ID_Fabrika { get; set; }          // Çelësi kryesor
        public string EmriFabrikes { get; set; } = "";  // Emri i shkollës
        public string Lokacioni { get; set; } = "";        // Qyteti ku ndodhet

        // Navigation property — lista e nxënësve që i përkasin kësaj shkolle
        // "Një shkollë ka SHUMË nxënës"
        public List<Punetori> Punetoret { get; set; } = new List<Punetori>();

    }
}
