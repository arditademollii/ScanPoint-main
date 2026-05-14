using ScanPoint.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace ScanPoint.Models.Models
{
    // Tabela e shkollës — entity kryesore
    public class Shkolla
    {//key per me dite qe eshte primary key
        [Key]
        public int ID_Shkolla { get; set; }          // Çelësi kryesor
        public string EmriShkolles { get; set; } = "";  // Emri i shkollës
        public string Qyteti { get; set; } = "";        // Qyteti ku ndodhet

        // Navigation property — lista e nxënësve që i përkasin kësaj shkolle
        // "Një shkollë ka SHUMË nxënës"
        public List<Nxenesi> Nxenesit { get; set; } = new List<Nxenesi>();

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
