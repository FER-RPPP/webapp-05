﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Display(Name = "OIB")]
        [Required(ErrorMessage = "Potrebno je unjeti OIB")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "OIB mora sadržavati točno 11 znamenki.")]
        [StringLength(11, ErrorMessage = "OIB mora imati točno 11 znakova.")]
        public string Oib { get; set; }

        [Display(Name = "Adresa")]
        [Required(ErrorMessage = "Potrebno je unjeti adresu")]
        [RegularExpression(@"^[a-zA-ZČčĆćŽžŠšĐđ]+(\s[a-zA-ZČčĆćŽžŠšĐđ]+)*\s\d+[A-Za-zČčĆćŽžŠšĐđ]?$", ErrorMessage = "Adresa treba početi riječima i sadržavati kućanski broj bez navođenja grada.")]

        public string AdresaPartner { get; set; }

        [Display(Name = "IBAN")]
        [Required(ErrorMessage = "Potrebno je unjeti IBAN")]
        [RegularExpression(@"^[A-Za-z]{2}\d{19}$", ErrorMessage = "IBAN mora početi s 2 slova i zatim imati 19 brojki.")]
        [StringLength(21, ErrorMessage = "IBAN mora imati točno 21 znak.")]
        public string Ibanpartner { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Potrebno je unjeti Email adresu.")]
        [EmailAddress(ErrorMessage = "Email adresa nije validna.")]
        [RegularExpression(@"^[a-zA-Z0-9ČčĆćŽžŠšĐđ._%+-]+[._-]?[a-zA-Z0-9ČčĆćŽžŠšĐđ._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email adresa nije validna.")]

        public string EmailPartner { get; set; }
        public string NazivPartner { get; set; }
        public int IdTipPartnera { get; set; }

        public virtual TipPartnera IdTipPartneraNavigation { get; set; }
        public virtual ICollection<Suradnik> Suradnik { get; set; }

        public virtual ICollection<Projekt> IdProjekt { get; set; }

    }
}