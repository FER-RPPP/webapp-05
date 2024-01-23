using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// Prikaz modela s podacima o poslovima
    /// </summary>
    public class PosaoViewModel
    {
        /// <summary>
        /// Predstavlja popis poslova koji se prikazuju
        /// </summary>
        public List<Posao> poslovi { get; set; }

        /// <summary>
        /// Predstavlja popis vrsta poslova koji se prikazuju
        /// </summary>
        public List<string> vrstaPosla { get; set; }

        /// <summary>
        /// Predstavlja popis suradnika koji rade na poslu
        /// </summary>
        public List<string> suradnici { get; set; }

        /// <summary>
        /// Predstavja informacija o straničenju
        /// </summary>
        public PagingInfo PagingInfo { get; set; }

    }
}
