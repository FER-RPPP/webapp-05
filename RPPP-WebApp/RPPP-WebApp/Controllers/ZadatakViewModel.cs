using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// ViewModel za prikaz zadataka, koristi se u jednostavnom prikazu zadataka.
    /// </summary>
    public class ZadatakViewModel
    {
        /// <summary>
        /// Dohvaća ili postavlja listu zadataka.
        /// </summary>
        public List<Zadatak> zadatci { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja listu naziva statusa.
        /// </summary>
        public List<string> nazivStatusa { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja informacije o straničenju za prikaz zadataka.
        /// </summary>
        public PagingInfo PagingInfo { get; set; }
    }

}
