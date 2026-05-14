using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.DTOs
{
    public class PunetoriCreateDto
    {
        [Required(ErrorMessage = "Emri është i detyrueshëm")]
        public string EmriPunetorit { get; set; } = "";

        [Required(ErrorMessage = "Mbiemri është i detyrueshëm")]
        public string MbiemriPunetorit { get; set; } = "";

        [Required(ErrorMessage = "Pozita është e detyrueshme")]
        public string Pozita { get; set; } = "";

        // ID e shkollës ku regjistrohet nxënësi
        [Range(1, int.MaxValue, ErrorMessage = "Fabrika duhet zgjedhur")]
        public int ID_Fabrika { get; set; }
    }

    // DTO për LEXIM — kthejmë edhe emrin e shkollës (jo vetëm ID-në)
    public class PunetoriReadDto
    {
        public int ID { get; set; }
        public string EmriPunetorit { get; set; } = "";
        public string MbiemriPunetorit { get; set; } = "";
        public string Pozita { get; set; } = "";
        public int ID_Fabrika { get; set; }
        public string EmriFabrikes { get; set; } = "";  // Nga JOIN me fabrikën 
    }

    // DTO për PËRDITËSIM
    public class PunetoriUpdateDto
    {
        [Required]
        public string EmriPunetorit { get; set; } = "";

        [Required]
        public string MbiemriPunetorit { get; set; } = "";

        [Required]
        public string Pozita { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Fabrika duhet zgjedhur")]
        public int ID_Fabrika { get; set; }
    }
}
