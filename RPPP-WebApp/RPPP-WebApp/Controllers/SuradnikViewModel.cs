using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.Controllers
{
    public class SuradnikViewModel
    {
        public List<Suradnik> suradnici { get; set; }

        public List<string> kvalifikacija { get; set; }

        public List<string> poslovi {  get; set; }

        public PagingInfo PagingInfo { get; set; }

    }
}
