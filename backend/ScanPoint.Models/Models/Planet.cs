using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.Models
{
    public class Planet
    {
        [Key]
        public int ID_Planet { get; set; }

        public string EmriPlanetit { get; set; } = "";

        public string Type { get; set; } = "";

        public bool IsDeleted { get; set; } = false;

        public List<Satelite> Satelitet { get; set; } = new();
    }
}