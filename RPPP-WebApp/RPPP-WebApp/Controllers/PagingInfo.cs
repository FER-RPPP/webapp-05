using System;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// Predstavlja informacije o stranicenju podataka
    /// </summary>
    public class PagingInfo
    {
        /// <summary>
        /// Ukupan broj stavki
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Broj stavki po stranici
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Trenutna stranica
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Vraca ukupan broj stranica na temelju ukupnog broja stavki i broja stavki po stranici
        /// </summary>
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
            }
        }

        /// <summary>
        /// Postavlja redoslijed sortiranja uzlazno/silazno
        /// </summary>
        public bool Ascending { get; set; }

        /// <summary>
        /// Postavlja vrstu sortiranja.
        /// </summary>
        public int Sort { get; set; }
    }

}
