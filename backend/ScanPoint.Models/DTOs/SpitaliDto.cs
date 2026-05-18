using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{


    public class SpitaliCreateDto
    {
        [Required(ErrorMessage = "Emri i spitalit është i detyrueshëm")]
        public string EmriSpitalit { get; set; } = "";

        [Required(ErrorMessage = "Numri i kateve është i detyrueshëm")]
        public int NumriKateve { get; set; } = 0;

        [Required(ErrorMessage = "Ka urgjencë është i detyrueshëm")]
        public bool KaUrgjence { get; set; } = false;

        [Required(ErrorMessage = "Data e hapjes është e detyrueshme")]
        public DateTime DataHapjes { get; set; }
    }

  
    public class SpitaliReadDto
    {
        public int ID_Spitali { get; set; }
        public string EmriSpitalit { get; set; } = "";
        public int NumriKateve { get; set; } = 0;

        public bool KaUrgjence { get; set; } = false;

        public DateTime DataHapjes { get; set; }    
    }
}
