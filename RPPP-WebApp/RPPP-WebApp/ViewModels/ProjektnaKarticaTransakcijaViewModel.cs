using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using System.ComponentModel.DataAnnotations;
using RPPP_WebApp.Controllers;

namespace RPPP_WebApp.ViewModels
{
    /// <summary>
    /// stringagregate
    /// </summary>
    public class ProjektnaKarticaTransakcijaViewModel
    {
        public ProjektnaKartica kartica { get; set; }
        //public string NazVrsta { get; set; }/*POTENCIJALNO POPRAVIT*/
        public int? IdPrethKartica { get; set; }
        public int? IdSljedKartica { get; set; }

        public TransakcijaViewModel transakcije { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
