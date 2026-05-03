using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{
    // DTO = Data Transfer Object
    // Përdoret për të marrë të dhëna nga frontend-i kur SHTOJMË shkollë të re

    public class ShkollaCreateDto
    {
        [Required(ErrorMessage = "Emri i shkollës është i detyrueshëm")]
        public string EmriShkolles { get; set; } = "";

        [Required(ErrorMessage = "Qyteti është i detyrueshëm")]
        public string Qyteti { get; set; } = "";
    }

    // DTO për LEXIM — kthejmë këtë te frontend-i
    public class ShkollaReadDto
    {
        public int ID_Shkolla { get; set; }
        public string EmriShkolles { get; set; } = "";
        public string Qyteti { get; set; } = "";
    }
}
