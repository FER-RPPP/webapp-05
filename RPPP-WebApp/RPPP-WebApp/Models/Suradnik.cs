﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class Suradnik
    {
        public Suradnik()
        {
            IdPosaos = new HashSet<Posao>();
        }

        public string Oib { get; set; }
        public string Mobitel { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Mail { get; set; }
        public string Stranka { get; set; }
        public int IdSuradnik { get; set; }
        public int IdKvalifikacija { get; set; }
        public int IdPartner { get; set; }

        public virtual Kvalifikacija IdKvalifikacijaNavigation { get; set; }
        public virtual Partner IdPartnerNavigation { get; set; }

        public virtual ICollection<Posao> IdPosaos { get; set; }
    }
}