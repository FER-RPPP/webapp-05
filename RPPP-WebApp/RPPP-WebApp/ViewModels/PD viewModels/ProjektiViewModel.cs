using RPPP_WebApp.Controllers;
using RPPP_WebApp.Models;


namespace RPPP_WebApp.ViewModels
{
    /// <summary>
    /// View model za projekte
    /// </summary>
    public class ProjektiViewModel
    {
        /// <summary>
        /// Skup projekata
        /// </summary>
        public IEnumerable<Projekt> Projekti { get; set; }
        /// <summary>
        /// Paging info
        /// </summary>
        public PagingInfo PagingInfo { get; set; }
        /// <summary>
        /// Lisa vrsta projekata
        /// </summary>
        public List<string> vrstaProjekataList { get; set; }
        /// <summary>
        /// Lista povezanih dokumenata
        /// </summary>
        public List<string> povezaniDokumenti { get; set; }
    }
}
