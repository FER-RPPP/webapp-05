﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class Transakcija
    {
        public string Opis { get; set; }
        public string PrimateljIban { get; set; }
        public string Vrsta { get; set; }
        public string SubjektIban { get; set; }
        public int IdTransakcije { get; set; }

        public virtual VrstaTransakcije IdTransakcijeNavigation { get; set; }
        public virtual ProjektnaKartica SubjektIbanNavigation { get; set; }
    }
}