﻿
﻿using RPPP_WebApp.Models;
using RPPP_WebApp.ModelsValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Controllers
{
    public class SuradnikPomocniViewModel
    {
        [Display(Name = "OIB")]
        [Required(ErrorMessage = "Potrebno je unjeti OIB")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "OIB mora sadržavati točno 11 znamenki.")]
        [StringLength(11, ErrorMessage = "OIB mora imati točno 11 znakova.")]
        public string Oib { get; set; }

        [Display(Name = "Broj mobitela")]
        [Required(ErrorMessage = "Potrebno je unjeti broj mobitela")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Broj mobitela mora sadržavati točno 10 znamenki.")]
        [StringLength(10, ErrorMessage = "Broj mobitela mora imati točno 10 znakova.")]
        public string Mobitel { get; set; }

        [Display(Name = "Ime")]
        [Required(ErrorMessage = "Potrebno je unjeti ime suradnika")]
        [RegularExpression(@"^[a-zA-ZČčĆćŽžŠšĐđ]+$", ErrorMessage = "Ime može imati samo slova.")]
        public string Ime { get; set; }

        [Display(Name = "Prezime")]
        [Required(ErrorMessage = "Potrebno je unjeti prezime suradnika")]
        [RegularExpression(@"^[a-zA-ZČčĆćŽžŠšĐđ]+$", ErrorMessage = "Prezime može imati samo slova.")]
        public string Prezime { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Potrebno je unjeti Email adresu.")]
        [EmailAddress(ErrorMessage = "Email adresa nije validna.")]
        [RegularExpression(@"^[a-zA-Z0-9ČčĆćŽžŠšĐđ._%+-]+[._-]?[a-zA-Z0-9ČčĆćŽžŠšĐđ._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email adresa nije validna.")]
        public string Mail { get; set; }

        [Display(Name = "Stranka")]
        [Required(ErrorMessage = "Potrebno je unjeti stranku.")]
        public string Stranka { get; set; }
        public int IdSuradnik { get; set; }
        public int IdKvalifikacija { get; set; }
        public int IdPartner { get; set; }

        public virtual Kvalifikacija IdKvalifikacijaNavigation { get; set; }
        public virtual Partner IdPartnerNavigation { get; set; }

        public virtual ICollection<Posao> IdPosao { get; set; }
        public string NazivKvalifikacija { get; set; }
    }
}