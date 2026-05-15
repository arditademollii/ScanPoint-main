using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{
    // DTO = Data Transfer Object
    // Përdoret për të marrë të dhëna nga frontend-i kur SHTOJMË universitet të ri

    public class UniversitetiCreateDto
    {
        [Required(ErrorMessage = "Emri i universitetit është i detyrueshëm")]
        public string EmriUniversitetit { get; set; } = "";

        [Required(ErrorMessage = "Shteti është i detyrueshëm")]
        public string Shteti { get; set; } = "";
    }

    // DTO për LEXIM — kthejmë këtë te frontend-i
    public class UniversitetiReadDto
    {
        public int ID_Universiteti { get; set; }
        public string EmriUniversitetit { get; set; } = "";
        public string Shteti { get; set; } = "";
    }
}
