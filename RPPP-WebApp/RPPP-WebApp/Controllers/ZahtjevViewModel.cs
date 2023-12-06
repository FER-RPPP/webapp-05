using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.Controllers
{
    public class ZahtjevViewModel
    {
        public List<Zahtjev> zadatci { get; set; }
        public List<string> nazivVrste { get; set; }

        public PagingInfo PagingInfo { get; set; }

    }
}
