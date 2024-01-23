using RPPP_WebApp.Models;
using RPPP_WebApp.ModelsValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// ViewModel za prikaz suradnika(kad je suradnika detail)
    /// Koristi se za azuriranje u MD formi
    /// </summary>
    public class SuradnikDetailPomocniViewModel
    {
        /// <summary>
        /// Dohvaca ili postavlja OIB suradnika
        /// </summary>
        [Display(Name = "OIB")]
        [Required(ErrorMessage = "Potrebno je unjeti OIB")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "OIB mora sadržavati točno 11 znamenki.")]
        [StringLength(11, ErrorMessage = "OIB mora imati točno 11 znakova.")]
        public string Oib { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja broj mobitela suradnika
        /// </summary>
        [Display(Name = "Broj mobitela")]
        [Required(ErrorMessage = "Potrebno je unjeti broj mobitela")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Broj mobitela mora sadržavati točno 10 znamenki.")]
        [StringLength(10, ErrorMessage = "Broj mobitela mora imati točno 10 znakova.")]
        public string Mobitel { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja ime suradnika
        /// </summary>
        [Display(Name = "Ime")]
        [Required(ErrorMessage = "Potrebno je unjeti ime suradnika")]
        [RegularExpression(@"^[a-zA-ZČčĆćŽžŠšĐđ]+$", ErrorMessage = "Ime može imati samo slova.")]
        public string Ime { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja prezime suradnika
        /// </summary>
        [Display(Name = "Prezime")]
        [Required(ErrorMessage = "Potrebno je unjeti prezime suradnika")]
        [RegularExpression(@"^[a-zA-ZČčĆćŽžŠšĐđ]+$", ErrorMessage = "Prezime može imati samo slova.")]
        public string Prezime { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Potrebno je unjeti Email adresu.")]
        [EmailAddress(ErrorMessage = "Email adresa nije validna.")]
        [RegularExpression(@"^[a-zA-Z0-9ČčĆćŽžŠšĐđ._%+-]+[._-]?[a-zA-Z0-9ČčĆćŽžŠšĐđ._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email adresa nije validna.")]
        public string Mail { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja stranku suradnika
        /// </summary>
        [Display(Name = "Stranka")]
        [Required(ErrorMessage = "Potrebno je unjeti stranku.")]
        public string Stranka { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja identifiakator (Id) suradnika
        /// </summary>
        public int IdSuradnik { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja identifikator (Id) kvalifikacije suradnika
        /// </summary>
        public int IdKvalifikacija { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja identifikator (Id) partnera povezanog s odredenim suradnikom
        /// </summary>
        public int IdPartner { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja objekt kvalifikacije povezane sa suradnikom
        /// Koristi drugu tablicu baze podataka
        /// </summary>
        public virtual Kvalifikacija IdKvalifikacijaNavigation { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja objekt partnera povezanog s odredenim suradnikom
        /// Korisni drugu tablicu iz baze podataka
        /// </summary>
        public virtual Partner IdPartnerNavigation { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja identifikator (Id) posla
        /// </summary>
        public virtual ICollection<Posao> IdPosao { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja naziv kvalifikacije suradnika
        /// </summary>
        public string NazivKvalifikacija { get; set; }
    }
}