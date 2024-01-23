using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.Controllers
{

    /// <summary>
    /// Prikaz modela koji objedinjuje podatke o suradniku i njegovim poslovima
    /// </summary>
    public class SuradnikPosaoViewModel
    {
        /// <summary>
        /// Predstavlja suradnika
        /// </summary>
        public Suradnik suradnik {  get; set; }

        /// <summary>
        /// Predstavlja suradnikovu kvalifikaciju
        /// </summary>
        public string Kvalifikacija { get; set; }

        /// <summary>
        /// Predstavlja identifikacijski broj prethodnog suradnika
        /// </summary>
        public int? IdPrethSuradnik { get; set; }

        /// <summary>
        /// Predstavlja identifikacijski broj sljedeceg suradnika
        /// </summary>
        public int? IdSljedSuradnik { get; set; }

        /// <summary>
        /// Predstavlja popis poslova koje suradnik radi
        /// </summary>
        public PosaoViewModel poslovi {  get; set; }

        /// <summary>
        /// Predstavlja informacije o stranicenju
        /// </summary>
        public PagingInfo PagingInfo { get; set; }

    }
}
