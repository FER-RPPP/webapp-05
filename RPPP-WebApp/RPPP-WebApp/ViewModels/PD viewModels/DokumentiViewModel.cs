using RPPP_WebApp.Controllers;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    /// <summary>
    /// View model za dokumente
    /// </summary>
    public class DokumentiViewModel
    {
        /// <summary>
        /// popis dokumenata
        /// </summary>
        public IEnumerable<Dokument> Dokumenti { get; set; }

        /// <summary>
        /// Paging info
        /// </summary>
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// Popis vrsta dokumenata
        /// </summary>
        public List<string> VrstaDokumenta { get; set; }

        /// <summary>
        /// Popis povezanih projekata
        /// </summary>
        public List<Projekt> povezaniProjekti { get; set; }
    }
}
