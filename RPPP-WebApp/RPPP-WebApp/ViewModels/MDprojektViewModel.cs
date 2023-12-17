using RPPP_WebApp.Controllers;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class MDprojektViewModel
    {
        public Projekt projekt { get; set; }
        public string TipProjekta { get; set; }
        public int? IdPrethProjekt { get; set; }
        public int? IdSljedProjekt { get; set; }

        public DokumentiViewModel dokumenti { get; set; }

        public PagingInfo PagingInfo { get; set; }

    }
}
