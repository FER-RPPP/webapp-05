﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class Uloga
    {
        public Uloga()
        {
            Imas = new HashSet<Ima>();
        }

        public int IdUloga { get; set; }
        public string NazivUloge { get; set; }
        public int IdProjekt { get; set; }

        public virtual Projekt IdProjektNavigation { get; set; }
        public virtual ICollection<Ima> Imas { get; set; }
    }
}