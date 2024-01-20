﻿using RPPP_WebApp.Models;
using RPPP_WebApp.ModelsValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Controllers
{
    public class ZahtjevPomocniViewModel
    {

        [Display(Name = "Opis zahtjeva")]
        [Required(ErrorMessage = "Potrebno je napisati opis zahtjeva")]
        [StringLength(100, ErrorMessage = "Opis ima maksimalno 100 znakova.")]
        public string Opis { get; set; }
        [Required(ErrorMessage = "Potrebno je unijeti prioritet")]
        [Display(Name = "Prioritet")]
        [StringLength(20, ErrorMessage = "Prioritet ima maksimalno 20 znakova.")]
        public string Prioritet { get; set; }

        public int IdZahtjev { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Datum")]
        [DateGreaterThan(nameof(VrPocetak), ErrorMessage = "Vrijeme kraja mora biti nakon vremena početka")]
        public DateTime? VrKraj { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Datum")]
        [Required(ErrorMessage = "Potrebno je odabrati datum ")]
        [DateGreaterThan(nameof(VrPocetak), ErrorMessage = "Očekivano vrijeme kraja mora biti nakon vremena početka")]
        public DateTime VrKrajOcekivano { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Datum")]
        [Required(ErrorMessage = "Potrebno je odabrati datum ")]
        public DateTime VrPocetak { get; set; }
        [Display(Name = "Vrsta")]
        [Required(ErrorMessage = "Potrebno je odabrati vrstu")]
        public int IdVrsta { get; set; }
        [Display(Name = "IdProjekt")]
        [Required(ErrorMessage = "Potrebno je odabrati projekt")]
        public int IdProjekt { get; set; }

        public virtual Projekt IdProjektNavigation { get; set; }
        public virtual VrstaZahtjeva IdVrstaNavigation { get; set; }
        public virtual ICollection<Zadatak> Zadatak { get; set; }

        public string NazivVrsta { get; set; }

    }
}
