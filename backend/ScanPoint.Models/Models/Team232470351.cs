using ScanPoint.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.Models
{
    // Tabela e shkollës — entity kryesore
    public class Team232470351
    {//key per me dite qe eshte primary key
        [Key]
        public int ID_Team232470351 { get; set; }          // Çelësi kryesor
        public string EmriTeam232470351 { get; set; } = "";  // Emri i shkollës
        

        // Navigation property — lista e nxënësve që i përkasin kësaj shkolle
        // "Një shkollë ka SHUMË nxënës"
        public List<Player232470351> Players232470351 { get; set; } = new List<Player232470351>();

        //kena mujt me perdore dhe ICollection<Nxenesi> ose IEnumerable<Nxenesi>




        /* 1me1
        Navigation property
        public Pasaporta Pasaporta { get; set; } = null!;
            dhe 
        // Navigation property
    public Personi Personi { get; set; } = null!;


        NmeN
         // Shumë lëndë
    public List<Lenda> Lendet { get; set; } = new();
           dhe
    // Shumë studentë
    public List<Studenti> Studentet { get; set; } = new();



        */
    }
}
