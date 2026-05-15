using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{
    // DTO = Data Transfer Object
    // Përdoret për të marrë të dhëna nga frontend-i kur SHTOJMË punonjës të ri

    public class EmployeeCreateDto
    {
        [Required(ErrorMessage = "Emri i employee është i detyrueshëm")]
        public string EmriEmployee { get; set; } = "";

        [Required(ErrorMessage = "Mbiemri i employee është i detyrueshëm")]
        public string MbiemriEmployee { get; set; } = "";
    }

    // DTO për LEXIM — kthejmë këtë te frontend-i
    public class EmployeeReadDto
    {
        public int ID_Employee { get; set; }
        public string EmriEmployee { get; set; } = "";
        public string MbiemriEmployee { get; set; } = "";
    }
}
