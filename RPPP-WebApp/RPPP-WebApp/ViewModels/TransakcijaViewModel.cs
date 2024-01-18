using RPPP_WebApp.Models;
using RPPP_WebApp.Controllers;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
    public class TransakcijaViewModel
    {
        public IEnumerable<Transakcija> Transakcija { get; set; }
        public List<string> nazivTransakcija { get; set; }
        public PagingInfo PagingInfo { get; set; }

        internal object Where(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
    }
}