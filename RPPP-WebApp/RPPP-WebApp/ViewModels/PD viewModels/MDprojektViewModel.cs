using RPPP_WebApp.Controllers;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    /// <summary>
    /// Pomocni view model za MD prikaz projekata i povezanih dokumenata
    /// </summary>
    public class MDprojektViewModel
    {
        /// <summary>
        /// Projekt
        /// </summary>
        public Projekt projekt { get; set; }
        /// <summary>
        /// Tip projekta
        /// </summary>
        public string TipProjekta { get; set; }
        /// <summary>
        /// Prethodni projekt
        /// </summary>
        public int? IdPrethProjekt { get; set; }
        /// <summary>
        /// Sljedeci projekt
        /// </summary>
        public int? IdSljedProjekt { get; set; }

        /// <summary>
        /// Paging info
        /// </summary>
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// Popis poveznih dokumenata
        /// </summary>
        public IEnumerable<DokPomViewModel> Dokumenti { get; set;}

        /// <summary>
        /// konstruktor
        /// </summary>
        public MDprojektViewModel()
        {
            this.Dokumenti = new List<DokPomViewModel>();
        }

    }
}
