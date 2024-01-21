using RPPP_WebApp.Controllers;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.ViewModels
{
    /// <summary>
    /// Prikaz modela koji agregira podatke o projektnoj kartici i transakcijama
    /// </summary>
    public class ProjektnaKarticaTransakcijaViewModel
    {
        /// <summary>
        /// Dohvaca ili postavlja podatke o projektnoj kartici
        /// </summary>
        public ProjektnaKartica kartica { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja ID prethodne projektna kartice
        /// </summary>
        public int? IdPrethKartica { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja ID sljedece projektna kartice
        /// </summary>
        public int? IdSljedKartica { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja prikaz modela za transakcije povezane s projektnom karticom
        /// </summary>
        public TransakcijaViewModel transakcije { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja informacije o stranicenju
        /// </summary>
        public PagingInfo PagingInfo { get; set; }
    }
}
