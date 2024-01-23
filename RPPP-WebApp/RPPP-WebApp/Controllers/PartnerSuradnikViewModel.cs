using RPPP_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// Prikaz MD forme partnera i suradnika
    /// </summary>
    public class PartnerSuradnikViewModel
    {
        /// <summary>
        /// Dohvaca ili postavlja kolekciju partnera
        /// </summary>
        public Partner partner { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja tip partnera
        /// </summary>
        public string TipPartnera { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja Id prethodnog partnera
        /// </summary>
        public int? IdPrethPartner { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja Id sljedeceg partnera
        /// </summary>
        public int? IdSljedPartner { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja kolekciju suradnika
        /// </summary>
        public IEnumerable<SuradnikDetailPomocniViewModel> Suradnici { get; set; }

        /// <summary>
        /// Lista obrisanih podataka
        /// </summary>
        public List<int> brisani { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja listu kvalifikacija suradnika
        /// </summary>
        public List<string> Kvalifikacije { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja objekt Suradnik ViewModel-a
        /// </summary>
        public SuradnikDetailViewModel suradnici { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja informacije o stranicenju
        /// </summary>
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// Novi objjekt PartnerSuradnikViewModel
        /// Inicjalna vrijednost Suradnici je postavljena na praznu listu koja ima elemente 
        /// tipa SuradnikDetailPomocniViewModel
        /// </summary>
        public PartnerSuradnikViewModel()
        {
            this.Suradnici = new List<SuradnikDetailPomocniViewModel>();
        }
    }
}
