using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.Controllers
{
    public class SuradnikDetailViewModel
    {
        public List<Suradnik> suradnici { get; set; }

        public List<string> nazivKvalifikacije { get; set; }

        public PagingInfo PagingInfo { get; set; }

    }
}