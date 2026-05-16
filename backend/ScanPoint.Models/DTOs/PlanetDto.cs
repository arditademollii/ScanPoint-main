using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{
    public class PlanetCreateDto
    {
        [Required]
        public string EmriPlanetit { get; set; } = "";

        [Required]
        public string Type { get; set; } = "";
    }

    public class PlanetReadDto
    {
        public int ID_Planet { get; set; }

        public string EmriPlanetit { get; set; } = "";

        public string Type { get; set; } = "";

        public bool IsDeleted { get; set; }
    }
}