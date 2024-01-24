using RPPP_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.ViewModels
{
    /// <summary>
    /// Popocni ViewModel za dokument
    /// </summary>
    public class DokPomViewModel
    {
        /// <summary>
        /// Id dokumenta
        /// </summary>
        public int IdDokument { get; set; }

        /// <summary>
        /// Ekstenzija dokumenta
        /// </summary>
        [Display(Name = "Ekstenzija dokumenta")]
        [Required(ErrorMessage = "Potrebno je izabrati ekstenziju dokumenta")]
        public string TipDokument { get; set; }

        /// <summary>
        /// Veličina dokumenta
        /// </summary>
        [Display(Name = "Veličina dokumenta")]
        [Required(ErrorMessage = "Potrebno je odrediti veličinu dokumenta")]
        public string VelicinaDokument { get; set; }


        /// <summary>
        /// ID pripdanog projekta
        /// </summary>
        [Display(Name = "Pripadni projekt")]
        [Required(ErrorMessage = "Potrebno je izabrati kojem projektu pripada dokument")]
        public int IdProjekt { get; set; }

        /// <summary>
        /// ID vrste dokumenta
        /// </summary>
        [Display(Name = "Vrsta dokumenta")]
        [Required(ErrorMessage = "Potrebno je izabrati vrstu dokumenta")]
        public int IdVrstaDok { get; set; }

        /// <summary>
        /// Naziv dokumenta
        /// </summary>
        [Display(Name = "Naziv Dokumenta")]
        [Required(ErrorMessage = "Potrebno je napisati naziv dokumenta")]
        public string NazivDatoteka { get; set; }

        /// <summary>
        /// Lokacija dokumenta
        /// </summary>
        [Display(Name = "Lokacija dokumenta")]
        [Required(ErrorMessage = "Potrebno je napisati lokaciju dokumenta")]
        public string URLdokument { get; set; }

        /// <summary>
        /// Za navigiranje na povezani projekt
        /// </summary>
        public virtual Projekt IdProjektNavigation { get; set; }

        /// <summary>
        /// Za navigiranje na povezanu vrstu dokumenta
        /// </summary>
        public virtual VrstaDokumenta IdVrstaDokNavigation { get; set; }

        /// <summary>
        /// Naziv Vrste dokumenta
        /// </summary>
        public string NazivVrstaDok { get; set; }
    }
}