using RPPP_WebApp.Models;
using RPPP_WebApp.Controllers;
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
    /// <summary>
    /// ViewModel za prikaz transakcija
    /// </summary>
    public class TransakcijaViewModel
    {
        /// <summary>
        /// Dohvaca ili postavlja kolekciju transakcija
        /// </summary>
        public IEnumerable<Transakcija> Transakcija { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja nazive transakcija
        /// </summary>
        public List<string> nazivTransakcija { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja informacije o stranicenju
        /// </summary>
        public PagingInfo PagingInfo { get; set; }

        internal object Where(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
    }
}
