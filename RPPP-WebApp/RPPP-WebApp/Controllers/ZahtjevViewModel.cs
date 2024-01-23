using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// ViewModel za prikaz zahtjeva, korišten u jednostavnom prikazu zahtjeva.
    /// </summary>
    public class ZahtjevViewModel
    {
        /// <summary>
        /// Lista objekata Zahtjev koji se prikazuju.
        /// </summary>
        public List<Zahtjev> zahtjevi { get; set; }

        /// <summary>
        /// Lista naziva vrsti zahtjeva.
        /// </summary>
        public List<string> nazivVrste { get; set; }

        /// <summary>
        /// Informacije o straničenju i sortiranju za prikaz.
        /// </summary>
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// Popis zadataka povezanih s određenim zahtjevom.
        /// </summary>
        public List<string> popisZadataka { get; set; }
    }

}
