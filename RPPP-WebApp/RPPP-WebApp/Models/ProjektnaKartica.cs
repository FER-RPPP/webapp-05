﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class ProjektnaKartica
    {
        public ProjektnaKartica()
        {
            Transakcijas = new HashSet<Transakcija>();
        }

        public double Saldo { get; set; }
        public string SubjektIban { get; set; }
        public string Valuta { get; set; }
        public DateTime VrijemeOtvaranja { get; set; }
        public int IdProjekt { get; set; }

        public virtual Projekt IdProjektNavigation { get; set; }
        public virtual ICollection<Transakcija> Transakcijas { get; set; }
    }
}