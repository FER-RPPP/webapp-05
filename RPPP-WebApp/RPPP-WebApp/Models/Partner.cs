﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class Partner
    {
        public Partner()
        {
            Suradnik = new HashSet<Suradnik>();
            IdProjekt = new HashSet<Projekt>();
        }

        public int IdPartner { get; set; }
        public string Oib { get; set; }
        public string AdresaPartner { get; set; }
        public string Ibanpartner { get; set; }
        public string EmailPartner { get; set; }
        public string NazivPartner { get; set; }
        public int IdTipPartnera { get; set; }

        public virtual TipPartnera IdTipPartneraNavigation { get; set; }
        public virtual ICollection<Suradnik> Suradnik { get; set; }

        public virtual ICollection<Projekt> IdProjekt { get; set; }
    }
}