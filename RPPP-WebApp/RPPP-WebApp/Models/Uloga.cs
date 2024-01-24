﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    /// <summary>
    /// Predstavlja ulogu suradnika u projektu
    /// </summary>
    public partial class Uloga
    {
        /// <summary>
        /// Inicijalizacija nove instance klase Uloga
        /// </summary>
        public Uloga()
        {
            Ima = new HashSet<Ima>();
        }

        /// <summary>
        /// Predstavlja identifikacijski broj uloge
        /// </summary>
        public int IdUloga { get; set; }

        /// <summary>
        /// Predstavlja naziv uloge
        /// </summary>
        public string NazivUloge { get; set; }

        /// <summary>
        /// Predstavlja identifikacijski broj projekta kojem pripada ta uloga
        /// </summary>
        public int IdProjekt { get; set; }

        /// <summary>
        /// Predstavlja navigacijski objekt prema objektu Projekt
        /// </summary>
        public virtual Projekt IdProjektNavigation { get; set; }

        /// <summary>
        /// Predstavlja popis suradnika koji imaju tu ulogu
        /// </summary>
        public virtual ICollection<Ima> Ima { get; set; }
    }
}