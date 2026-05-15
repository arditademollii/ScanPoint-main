using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{
    // DTO për SHTIM të nxënësit të ri
    public class ContractCreateDto
    {
        [Required(ErrorMessage = "Titulli është i detyrueshëm")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "Përshkrimi është i detyrueshëm")]
        public string Description { get; set; } = "";

        // ID e punonjësit që lidhet me kontratën
        [Range(1, int.MaxValue, ErrorMessage = "Punonjësi duhet zgjedhur")]
        public int ID_Employee { get; set; }
    }

    // DTO për LEXIM — kthejmë edhe emrin e punonjësit (jo vetëm ID-në)
    public class ContractReadDto
    {
        public int ID { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int ID_Employee { get; set; }
        public string EmriEmployee { get; set; } = "";  // Nga JOIN me punonjësin
    }

    // DTO për PËRDITËSIM
    public class ContractUpdateDto
    {
        [Required]
        public string Title { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Punonjësi duhet zgjedhur")]
        public int ID_Employee { get; set; }
    }
}
