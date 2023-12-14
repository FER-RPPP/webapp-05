using RPPP_WebApp.Controllers;
using RPPP_WebApp.Models;


namespace RPPP_WebApp.ViewModels
{
    public class ProjektiViewModel
    {
        public IEnumerable<Projekt> Projekti { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
