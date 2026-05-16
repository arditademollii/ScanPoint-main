using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{
    public class SateliteCreateDto
    {
        [Required]
        public string EmriSatelitit { get; set; } = "";

        [Range(1, int.MaxValue)]
        public int ID_Planet { get; set; }
    }

    public class SateliteReadDto
    {
        public int ID { get; set; }

        public string EmriSatelitit { get; set; } = "";

        public bool IsDeleted { get; set; }

        public int ID_Planet { get; set; }

        public string EmriPlanetit { get; set; } = "";
    }

    public class SateliteUpdateDto
    {
        [Required]
        public string EmriSatelitit { get; set; } = "";

        [Range(1, int.MaxValue)]
        public int ID_Planet { get; set; }
    }
}