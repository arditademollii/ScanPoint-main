using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.DTOs
{
 
    public class MjekuCreateDto
    {
        [Required(ErrorMessage = "Emri është i detyrueshëm")]
        public string EmriMjekut { get; set; } = "";

        [Required(ErrorMessage = "Paga është e detyrueshme")]
        public decimal Paga { get; set; } = 0;

        [Required(ErrorMessage = "Data e punësimit është e detyrueshme")]
        public DateTime DataPunesimit { get; set; }

        [Required(ErrorMessage = "Specjalizimi është i detyrueshëm")]
        public bool EshteSpecialist { get; set; }   

      
        [Range(1, int.MaxValue, ErrorMessage = "Spitali duhet zgjedhur")]
        public int ID_Spitali { get; set; }
    }

  
    public class MjekuReadDto
    {
        public int ID { get; set; }
        public string EmriMjekut { get; set; } = "";
        public decimal Paga { get; set; } = 0;

        public DateTime DataPunesimit { get; set; }
        public bool EshteSpecialist { get; set; }


        public int ID_Spitali { get; set; }
        public string EmriSpitalit { get; set; } = "";  
    }

   
    public class MjekuUpdateDto
    {
        [Required]
        public string EmriMjekut { get; set; } = "";

        [Required(ErrorMessage = "Paga është e detyrueshme")]
        public decimal Paga { get; set; } = 0;

        [Required(ErrorMessage = "Data e punësimit është e detyrueshme")]
        public DateTime DataPunesimit { get; set; }

        [Required(ErrorMessage = "Specjalizimi është i detyrueshëm")]
        public bool EshteSpecialist { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Spitali duhet zgjedhur")]
        public int ID_Spitali { get; set; }
    }
}
