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

        public IEnumerable<ZadatakPomocniViewModel> Zadatci { get; set; }

        public List<int> brisani { get; set; }
        public List<string> Statusi { get; set; }

        public ZadatakViewModel zadatci { get; set; }

        public PagingInfo PagingInfo { get; set; }
    public ZahtjevZadatakViewModel()
    {
        this.Zadatci = new List<ZadatakPomocniViewModel>();
    }
    }
    
}


