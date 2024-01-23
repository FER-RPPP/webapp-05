using RPPP_WebApp.Controllers;
using RPPP_WebApp.Models;


namespace RPPP_WebApp.ViewModels
{
    /// <summary>
    /// ViewModel klasa za prikaz projektnih kartica
    /// </summary>
    public class ProjektnaKarticaViewModel
    {
        /// <summary>
        /// Dohvaca ili postavlja kolekciju objekata ProjektnaKartica
        /// </summary>
        public IEnumerable<ProjektnaKartica> ProjektnaKartica { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja informacije o stranicenju
        /// </summary>
        public PagingInfo PagingInfo { get; set; }
    }
}
