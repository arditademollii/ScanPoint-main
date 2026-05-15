using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{
    // DTO = Data Transfer Object
    // Përdoret për të marrë të dhëna nga frontend-i kur SHTOJMË shkollë të re

    public class TeamCreateDto
    {
        [Required(ErrorMessage = "Emri i team-it është i detyrueshëm")]
        public string EmriTeam232470351 { get; set; } = "";

        
    }

    // DTO për LEXIM — kthejmë këtë te frontend-i
    public class TeamReadDto
    {
        public int ID_Team232470351 { get; set; }
        public string EmriTeam232470351 { get; set; } = "";
        
    }
}
