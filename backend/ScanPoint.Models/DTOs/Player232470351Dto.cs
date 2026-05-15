using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{
    // DTO për SHTIM të nxënësit të ri
    public class Player232470351CreateDto
    {
        [Required(ErrorMessage = "Emri është i detyrueshëm")]
        public string EmriPlayer232470351 { get; set; } = "";

        [Required(ErrorMessage = "Number është e detyrueshme")]
        public string Number { get; set; } = "";

        // ID e shkollës ku regjistrohet nxënësi
        [Range(1, int.MaxValue, ErrorMessage = "Team232470351 duhet zgjedhur")]
        public int ID_Team232470351 { get; set; }
    }

    // DTO për LEXIM — kthejmë edhe emrin e shkollës (jo vetëm ID-në)
    public class Player232470351ReadDto
    {
        public int ID { get; set; }
        public string EmriPlayer232470351 { get; set; } = "";
        public string Number { get; set; } = "";
        public int ID_Team232470351 { get; set; }
        public string EmriTeam232470351 { get; set; } = "";  // Nga JOIN me team-in
    }

    // DTO për PËRDITËSIM
    public class Player232470351UpdateDto
    {
        [Required]
        public string EmriPlayer232470351 { get; set; } = "";

        [Required]
        public string Number { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Team232470351 duhet zgjedhur")]
        public int ID_Team232470351 { get; set; }
    }
}
