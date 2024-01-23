using RPPP_WebApp.Models;
using System.Collections.Generic;


namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// Prikaz partnera
    /// Koristi se u prikazu svih partnera
    /// </summary>
    public class PartnerViewModel
    {
        /// <summary>
        /// Lista objekata tipa Partner koji se prikazuju
        /// </summary>
        public List<Partner> partneri { get; set; }

        /// <summary>
        /// Lista naziva vrsti partnera
        /// </summary>
        public List<string> nazivVrste { get; set; }

        /// <summary>
        /// Popis suradnika odredenog partnera
        /// </summary>
        public List<string> popisSuradnika { get; set; }    

        /// <summary>
        /// Informacije o stranicenju i sortiranju
        /// </summary>
        public PagingInfo PagingInfo { get; set; }

    }
}
