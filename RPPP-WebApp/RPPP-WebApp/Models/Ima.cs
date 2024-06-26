﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    /// <summary>
    /// Predstavlja vezu Suradnik Ima Ulogu
    /// </summary>
    public partial class Ima
    {
        /// <summary>
        /// Predstavlja OIB suradnika koji ima ulogu
        /// </summary>
        public int Oib { get; set; }
        /// <summary>
        /// Predstavlja identifikacijski broj uloge koju suradnik ima
        /// </summary>
        public int IdUloga { get; set; }

        /// <summary>
        /// Predstavlja navigacijski objekt prema objektu Uloga
        /// </summary>
        public virtual Uloga IdUlogaNavigation { get; set; }
    }
}