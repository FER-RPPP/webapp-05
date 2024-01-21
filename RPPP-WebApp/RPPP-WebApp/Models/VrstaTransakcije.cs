#nullable disable

using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    /// <summary>
    /// Predstavlja vrstu transakcije
    /// </summary>
    public partial class VrstaTransakcije
    {
        /// <summary>
        /// Dobiva ili postavlja identifikator transakcije
        /// </summary>
        public int IdTransakcije { get; set; }

        /// <summary>
        /// Dobiva ili postavlja naziv transakcije
        /// </summary>
        public string NazivTransakcije { get; set; }

        /// <summary>
        /// Dobiva ili postavlja kolekciju transakcija povezanih s ovom vrstom transakcije
        /// </summary>
        public virtual ICollection<Transakcija> Transakcija { get; set; }
    }
}
