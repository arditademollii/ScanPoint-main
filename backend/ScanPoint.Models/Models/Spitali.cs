using ScanPoint.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.Models
{
    // Tabela e shkollës — entity kryesore
    public class Spitali
    {//key per me dite qe eshte primary key
        [Key]
        public int ID_Spitali { get; set; }          // Çelësi kryesor
        public string EmriSpitalit { get; set; } = "";  // Emri i spitalit
        public int NumriKateve { get; set; } = 0;
        public bool KaUrgjence { get; set; } = false;// Qyteti ku ndodhet

        public DateTime DataHapjes { get; set; }


        // Navigation property — lista e pacientëve që i përkasin këtij spitali
        // "Një spital ka SHUMË pacientë"
        public List<Mjeku> Mjeket { get; set; } = new List<Mjeku>();

        //kena mujt me perdore dhe ICollection<Mjeku> ose IEnumerable<Mjeku>


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
