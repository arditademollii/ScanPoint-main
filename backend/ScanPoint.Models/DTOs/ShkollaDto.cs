using System.ComponentModel.DataAnnotations;
namespace ScanPoint.Models.DTOs { 
   
    public class ShkollaCreateDto  
    {       
        [Required(ErrorMessage = "Emri i shkollës është i detyrueshëm")]     
        public string EmriShkolles { get; set; } = "";      
        [Required(ErrorMessage = "Qyteti është i detyrueshëm")]   
        public string Qyteti { get; set; } = "";     }    
   
    public class ShkollaReadDto     {   
        public int ID_Shkolla { get; set; }     
        public string EmriShkolles { get; set; } = "";   
        public string Qyteti { get; set; } = "";     
    } } 