using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs
{
    public class FabrikaCreateDto
    {
        [Required(ErrorMessage = "Emri i fabrikes është i detyrueshëm")]
        public string EmriFabrikes { get; set; } = "";

        [Required(ErrorMessage = "Lokacioni është i detyrueshëm")]
        public string Lokacioni { get; set; } = "";
    }

    // DTO për LEXIM — kthejmë këtë te frontend-i
    public class FabrikaReadDto
    {
        public int ID_Fabrika { get; set; }
        public string EmriFabrikes { get; set; } = "";
        public string Lokacioni { get; set; } = "";
    }
}
