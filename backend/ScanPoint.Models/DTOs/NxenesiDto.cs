using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{
    // DTO për SHTIM të nxënësit të ri
    public class NxenesiCreateDto
    {
        [Required(ErrorMessage = "Emri është i detyrueshëm")]
        public string EmriNxenesit { get; set; } = "";

        [Required(ErrorMessage = "Klasa është e detyrueshme")]
        public string Klasa { get; set; } = "";

        // ID e shkollës ku regjistrohet nxënësi
        [Range(1, int.MaxValue, ErrorMessage = "Shkolla duhet zgjedhur")]
        public int ID_Shkolla { get; set; }
    }

    // DTO për LEXIM — kthejmë edhe emrin e shkollës (jo vetëm ID-në)
    public class NxenesiReadDto
    {
        public int ID { get; set; }
        public string EmriNxenesit { get; set; } = "";
        public string Klasa { get; set; } = "";
        public int ID_Shkolla { get; set; }
        public string EmriShkolles { get; set; } = "";  // Nga JOIN me shkollën
    }

    // DTO për PËRDITËSIM
    public class NxenesiUpdateDto
    {
        [Required]
        public string EmriNxenesit { get; set; } = "";

        [Required]
        public string Klasa { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Shkolla duhet zgjedhur")]
        public int ID_Shkolla { get; set; }
    }
}
