using RPPP_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// ViewModel za prikaz master-detail forme zahtjeva i zadataka
    /// </summary>
    public class ZahtjevZadatakViewModel
    {
        /// <summary>
        /// Dohvaca ili postavlja kolekciju zahtjeva
        /// </summary>
        public Zahtjev zahtjev { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja naziv vrste zahtjeva
        /// </summary>
        public string NazVrsta { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja id prethodnog zahtjeva
        /// </summary>
        public int? IdPrethZahtjev { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja id sljedeceg zahtjeva
        /// </summary>
        public int? IdSljedZahtjev { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja kolekciju zadataka uz pomoc ZadatakPomocniViewModel-a 
        /// </summary>
        public IEnumerable<ZadatakPomocniViewModel> Zadatci { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja listu statusa
        /// </summary>
        public List<string> Statusi { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja objek zadatak view modela
        /// </summary>
        public ZadatakViewModel zadatci { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja informacije o stranicenju
        /// </summary>
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// Inicijalizira novi objekt klase ZahtjevZadatakViewModel.
        /// Konstruktor postavlja inicijalnu vrijednost svojstva Zadatci na praznu listu ZadatakPomocniViewModel objekata.
        /// </summary>
        public ZahtjevZadatakViewModel()
    {
        this.Zadatci = new List<ZadatakPomocniViewModel>();
    }
    }
    
}


