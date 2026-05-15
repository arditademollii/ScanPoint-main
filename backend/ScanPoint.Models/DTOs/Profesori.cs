using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{
    // DTO për SHTIM të nxënësit të ri
    public class ProfesoriCreateDto
    {
        [Required(ErrorMessage = "Emri është i detyrueshëm")]
        public string EmriProfesorit { get; set; } = "";

        [Required(ErrorMessage = "Lenda është e detyrueshme")]
        public string Lenda { get; set; } = "";

        // ID e shkollës ku regjistrohet nxënësi
        [Range(1, int.MaxValue, ErrorMessage = "Universiteti duhet zgjedhur")]
        public int ID_Universiteti { get; set; }
    }

    // DTO për LEXIM — kthejmë edhe emrin e shkollës (jo vetëm ID-në)
    public class ProfesoriReadDto
    {
        public int ID { get; set; }
        public string EmriProfesorit { get; set; } = "";
        public string Lenda { get; set; } = "";
        public int ID_Universiteti { get; set; }
        public string EmriUniversitetit { get; set; } = "";  // Nga JOIN me universitetin
    }

    // DTO për PËRDITËSIM
    public class ProfesoriUpdateDto
    {
        [Required]
        public string EmriProfesorit { get; set; } = "";

        [Required]
        public string Lenda { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Universiteti duhet zgjedhur")]
        public int ID_Universiteti { get; set; }
    }
}
