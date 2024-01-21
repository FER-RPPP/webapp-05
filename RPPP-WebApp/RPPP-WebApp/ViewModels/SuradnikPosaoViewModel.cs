using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.Controllers
{
    public class SuradnikPosaoViewModel
    {
        public Suradnik suradnik {  get; set; }
        public string Kvalifikacija { get; set; }
        public int? IdPrethSuradnik { get; set; }
        public int? IdSljedSuradnik { get; set; }
        public PosaoViewModel poslovi {  get; set; }
        public PagingInfo PagingInfo { get; set; }

    }
}
