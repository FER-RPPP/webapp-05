using RPPP_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.ViewModels
{
    public class DokPomViewModel
    {
        public int IdDokument { get; set; }

        [Display(Name = "Ekstenzija dokumenta")]
        [Required(ErrorMessage = "Potrebno je izabrati ekstenziju dokumenta")]
        public string TipDokument { get; set; }

        [Display(Name = "Veličina dokumenta")]
        [Required(ErrorMessage = "Potrebno je odrediti veličinu dokumenta")]
        public string VelicinaDokument { get; set; }

        [Display(Name = "Pripadni projekt")]
        [Required(ErrorMessage = "Potrebno je izabrati kojem projektu pripada dokument")]
        public int IdProjekt { get; set; }

        [Display(Name = "Vrsta dokumenta")]
        [Required(ErrorMessage = "Potrebno je izabrati vrstu dokumenta")]
        public int IdVrstaDok { get; set; }

        [Display(Name = "Naziv Dokumenta")]
        [Required(ErrorMessage = "Potrebno je napisati naziv dokumenta")]
        public string NazivDatoteka { get; set; }

        [Display(Name = "Lokacija dokumenta")]
        [Required(ErrorMessage = "Potrebno je napisati lokaciju dokumenta")]
        public string URLdokument { get; set; }

        public virtual Projekt IdProjektNavigation { get; set; }
        public virtual VrstaDokumenta IdVrstaDokNavigation { get; set; }

        public string VrstaDok { get; set; }
    }
}