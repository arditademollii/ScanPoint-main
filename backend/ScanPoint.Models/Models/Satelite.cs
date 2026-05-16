using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.Models
{
    public class Satelite
    {
        [Key]
        public int ID { get; set; }

        public string EmriSatelitit { get; set; } = "";

        public bool IsDeleted { get; set; } = false;

        public int ID_Planet { get; set; }

        public Planet Planet { get; set; } = null!;
    }
}