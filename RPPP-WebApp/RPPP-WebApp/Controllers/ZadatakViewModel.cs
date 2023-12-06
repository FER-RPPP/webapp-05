using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.Controllers
{
    public class ZadatakViewModel
    {
        public List<Zadatak> zadatci { get; set; }

        public List<string> nazivStatusa { get; set; }

        public PagingInfo PagingInfo { get; set; }

    }
}
