using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.Controllers
{
    public class PosaoViewModel
    {
        public List<Posao> poslovi { get; set; }

        public List<string> vrstaPosla { get; set; }

        public PagingInfo PagingInfo { get; set; }

    }
}
