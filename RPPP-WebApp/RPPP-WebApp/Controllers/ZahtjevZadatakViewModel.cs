using RPPP_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// stringagregate
    /// </summary>
    public class ZahtjevZadatakViewModel
    {
        public Zahtjev zahtjev { get; set; }
        public string NazVrsta { get; set; }
        public int? IdPrethZahtjev { get; set; }
        public int? IdSljedZahtjev { get; set; }

        public ZadatakViewModel zadatci { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}

