using RPPP_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// stringagregate
    /// </summary>
    public class PartnerSuradnikViewModel
    {
        public Partner partner { get; set; }
        public string TipPartnera { get; set; }
        public int? IdPrethPartner { get; set; }
        public int? IdSljedPartner { get; set; }

        public IEnumerable<SuradnikDetailPomocniViewModel> Suradnici { get; set; }

        public List<int> brisani { get; set; }
        public List<string> Kvalifikacije { get; set; }

        public SuradnikDetailViewModel suradnici { get; set; }

        public PagingInfo PagingInfo { get; set; }
        public PartnerSuradnikViewModel()
        {
            this.Suradnici = new List<SuradnikDetailPomocniViewModel>();
        }
    }
}
