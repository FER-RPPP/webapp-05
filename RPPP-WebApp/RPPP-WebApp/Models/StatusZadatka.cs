﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class StatusZadatka
    {
        public StatusZadatka()
        {
            Zadatak = new HashSet<Zadatak>();
        }

        public int IdStatus { get; set; }
        public string NazivStatus { get; set; }

        public virtual ICollection<Zadatak> Zadatak { get; set; }
    }
}