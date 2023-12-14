﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using RPPP_WebApp.ModelsValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Models
{
    public partial class Zadatak
    {
        [DataType(DataType.DateTime)]
        [Display(Name = "Datum")]
        [DateGreaterThan(nameof(VrPoc), ErrorMessage = "Vrijeme kraja mora biti nakon vremena početka")]
        public DateTime? VrKraj { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Datum")]
        [Required(ErrorMessage = "Potrebno je odabrati datum ")]
        public DateTime VrPoc { get; set; }

        [Display(Name = "OibNositelj")]
        [Required(ErrorMessage = "Potrebno je napisati oib nositelja")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "oib mora sadržavati točno 11 znamenki.")]
        [StringLength(11, ErrorMessage = "Oib mora imati točno 11 znakova.")]
        public string Oibnositelj { get; set; }
        public int IdZadatak { get; set; }
        [Required(ErrorMessage = "Potrebno je unijeti vrstu")]
        [Display(Name = "Vrsta")]
        public string Vrsta { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Datum")]
        [Required(ErrorMessage = "Potrebno je odabrati datum ")]
        [DateGreaterThan(nameof(VrPoc), ErrorMessage = "Očekivano vrijeme kraja mora biti nakon vremena početka")]
        public DateTime VrKrajOcekivano { get; set; }
        public int IdZahtjev { get; set; }
        [Display(Name = "Status")]
        [Required(ErrorMessage = "Potrebno je odabrati status")]
        public int IdStatus { get; set; }

        public virtual StatusZadatka IdStatusNavigation { get; set; }
        public virtual Zahtjev IdZahtjevNavigation { get; set; }
    }
}