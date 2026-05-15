using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{
    // DTO për SHTIM të nxënësit të ri
    public class LigjerataCreateDto
    {
        [Required(ErrorMessage = "Emri është i detyrueshëm")]
        public string EmriLigjerates { get; set; } = "";

      

        // ID e ligjeruesit ku regjistrohet ligjerata
        [Range(1, int.MaxValue, ErrorMessage = "Ligjeruesi duhet zgjedhur")]
        public int ID_Ligjeruesi { get; set; }
    }

    // DTO për LEXIM — kthejmë edhe emrin e ligjeruesit (jo vetëm ID-në)
    public class LigjerataReadDto
    {
        public int ID { get; set; }
        public string EmriLigjerates { get; set; } = "";
        public int ID_Ligjeruesi { get; set; }
        public string EmriLigjeruesit { get; set; } = "";  // Nga JOIN me ligjeruesin
    }

    // DTO për PËRDITËSIM
    public class LigjerataUpdateDto
    {
        [Required]
        public string EmriLigjerates { get; set; } = "";

  
        [Range(1, int.MaxValue, ErrorMessage = "Ligjeruesi duhet zgjedhur")]
        public int ID_Ligjeruesi { get; set; }
    }
}
