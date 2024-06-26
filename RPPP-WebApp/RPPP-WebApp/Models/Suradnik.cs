﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Models
{
    /// <summary>
    /// Predstavlja suradnika
    /// </summary>
    public partial class Suradnik
    {
        /// <summary>
        /// Inicijalizacija nove instance klase Suradnik
        /// </summary>
        public Suradnik()
        {
            IdPosao = new HashSet<Posao>();
        }

        /// <summary>
        /// Predstavlja OIB suradnika
        /// </summary>
        [Display(Name = "OIB")]
        [Required(ErrorMessage = "Potrebno je unjeti OIB")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "OIB mora sadržavati točno 11 znamenki.")]
        [StringLength(11, ErrorMessage = "OIB mora imati točno 11 znakova.")]
        public string Oib { get; set; }

        /// <summary>
        /// Predstavlja broj mobitela suradnika
        /// </summary>
        [Display(Name = "Broj mobitela")]
        [Required(ErrorMessage = "Potrebno je unjeti broj mobitela")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Broj mobitela mora sadržavati točno 10 znamenki.")]
        [StringLength(10, ErrorMessage = "Broj mobitela mora imati točno 10 znakova.")]
        public string Mobitel { get; set; }

        /// <summary>
        /// Predstavlja ime suradnika
        /// </summary>
        [Display(Name = "Ime")]
        [Required(ErrorMessage = "Potrebno je unjeti ime suradnika")]
        [RegularExpression(@"^[a-zA-ZČčĆćŽžŠšĐđ]+$", ErrorMessage = "Ime može imati samo slova.")]
        public string Ime { get; set; }

        /// <summary>
        /// Predstavlja prezime suradnika 
        /// </summary>
        [Display(Name = "Prezime")]
        [Required(ErrorMessage = "Potrebno je unjeti prezime suradnika")]
        [RegularExpression(@"^[a-zA-ZČčĆćŽžŠšĐđ]+$", ErrorMessage = "Prezime može imati samo slova.")]
        public string Prezime { get; set; }

        /// <summary>
        /// Predstavlja email adresu suradnika
        /// </summary>
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Potrebno je unjeti Email adresu.")]
        [EmailAddress(ErrorMessage = "Email adresa nije validna.")]
        [RegularExpression(@"^[a-zA-Z0-9ČčĆćŽžŠšĐđ._%+-]+[._-]?[a-zA-Z0-9ČčĆćŽžŠšĐđ._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email adresa nije validna.")]
        public string Mail { get; set; }

        /// <summary>
        /// Predstavlja stranku kojoj suradnik pripada
        /// </summary>
        [Display(Name = "Stranka")]
        [Required(ErrorMessage = "Potrebno je unjeti stranku.")]
        public string Stranka { get; set; }

        /// <summary>
        /// Predstavlja identifikacijski broj suradnika
        /// </summary>
        public int IdSuradnik { get; set; }

        /// <summary>
        /// Predstavlja identifikacijski broj kvalifikacije koju ima suradnik
        /// </summary>
        public int IdKvalifikacija { get; set; }

        /// <summary>
        /// Predstavlja identifikacijski broj partnera kojem suradnik pripada
        /// </summary>
        public int IdPartner { get; set; }

        /// <summary>
        /// Predstavlja navigacijski objekt prema objektu Kvalifikacija, sluzi za dohvacanje naziva kvalifikacije 
        /// </summary>
        public virtual Kvalifikacija IdKvalifikacijaNavigation { get; set; }

        /// <summary>
        /// Predstavlja navigacijski objekt prema objektu Partner, sluzi za dohvacanje naziva partnera 
        /// </summary>
        public virtual Partner IdPartnerNavigation { get; set; }

        /// <summary>
        /// Predstavlja popis poslova koje radi suradnik
        /// </summary>
        public virtual ICollection<Posao> IdPosao { get; set; }
    }
}