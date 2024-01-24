using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// ViewModel za prikaz suradnika
    /// Koristi se u popisu suradnika (jednostavni prikaz)
    /// </summary>
    public class SuradnikDetailViewModel
    {
        /// <summary>
        /// Dohvaca ili postavlja listu suradnika
        /// </summary>
        public List<Suradnik> suradnici { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja listu naziva kvalifikacija
        /// </summary>
        public List<string> nazivKvalifikacije { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja informacije o stranicenju za prikaz suradnika
        /// </summary>
        public PagingInfo PagingInfo { get; set; }

    }
}