using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{
    // DTO = Data Transfer Object
    // Përdoret për të marrë të dhëna nga frontend-i kur SHTOJMË shkollë të re

    public class LigjeruesiCreateDto
    {
        [Required(ErrorMessage = "Emri i ligjeruesit është i detyrueshëm")]
        public string EmriLigjeruesit { get; set; } = "";

        [Required(ErrorMessage = "Departamenti është i detyrueshëm")]
        public string Departamenti { get; set; } = "";

        [Required(ErrorMessage = "Email është i detyrueshëm")]
        public string Email { get; set; } = "";
    }

    // DTO për LEXIM — kthejmë këtë te frontend-i
    public class LigjeruesiReadDto
    {
        public int ID_Ligjeruesi { get; set; }
        public string EmriLigjeruesit { get; set; } = "";
        public string Departamenti { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
