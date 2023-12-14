using RPPP_WebApp.Models;
using System.Collections.Generic;


namespace RPPP_WebApp.Controllers
{
    public class PartnerViewModel
    {
        public List<Partner> partneri { get; set; }
        public List<string> nazivVrste { get; set; }

        public List<string> popisSuradnika { get; set; }    

        public PagingInfo PagingInfo { get; set; }

    }
}
