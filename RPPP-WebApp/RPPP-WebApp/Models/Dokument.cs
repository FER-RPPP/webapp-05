﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class Dokument
    {
        public int IdDokument { get; set; }
        public string TipDokument { get; set; }
        public string VelicinaDokument { get; set; }
        public int IdProjekt { get; set; }
        public int IdVrstaDok { get; set; }
        public string NazivDatoteka { get; set; }

        public virtual Projekt IdProjektNavigation { get; set; }
        public virtual VrstaDokumenta IdVrstaDokNavigation { get; set; }
    }
}