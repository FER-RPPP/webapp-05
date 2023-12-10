using RPPP_WebApp.Models;
using RPPP_WebApp.Controllers;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
    public class TransakcijaViewModel
    {
        public IEnumerable<Transakcija> Transakcija { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}