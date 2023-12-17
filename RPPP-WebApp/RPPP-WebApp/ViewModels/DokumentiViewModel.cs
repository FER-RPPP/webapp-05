using RPPP_WebApp.Controllers;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class DokumentiViewModel
    {
        public IEnumerable<Dokument> Dokumenti { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public List<string> VrstaDokumenta { get; set; }
    }
}
