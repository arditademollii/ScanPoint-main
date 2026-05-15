using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.Models
{
    public class Universiteti
    {
       
        [Key]
        public int ID_Universiteti { get; set; }          // Çelësi kryesor
        public string EmriUniversitetit { get; set; } = "";  // Emri i universitetit
        public string Shteti { get; set; } = "";        // Qyteti ku ndodhet

        // Navigation property — lista e profesorëve që i përkasin këtij universiteti
        // "Një universitet ka SHUMË profesorë"
        public List<Profesori> Profesoret { get; set; } = new List<Profesori>();
    }
}
