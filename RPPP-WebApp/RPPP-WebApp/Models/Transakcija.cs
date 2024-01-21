#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Models
{
    /// <summary>
    /// Predstavlja transakciju
    /// </summary>
    public partial class Transakcija
    {
        /// <summary>
        /// Dohvaca ili postavlja opis transakcije
        /// </summary>
        [Display(Name = "Opis transakcije")]
        [Required(ErrorMessage = "Potrebno je napisati opis zahtjeva")]
        [StringLength(100, ErrorMessage = "Opis može sadržavati najviše 100 znakova.")]
        public string Opis { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja IBAN primatelja transakcije
        /// </summary>
        [Display(Name = "IBAN primatelja")]
        [Required(ErrorMessage = "Potrebno je napisati valjani IBAN")]
        [RegularExpression(@"^[A-Z]{2}\d{2}[A-Z0-9]{1,30}$", ErrorMessage = "Nevaljan format IBAN-a.")]
        public string PrimateljIban { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja vrstu transakcije
        /// </summary>
        [Display(Name = "Vrsta")]
        [Required(ErrorMessage = "Potrebno je odabrati vrstu (broj između 1-4)")]
        public string Vrsta { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja IBAN subjekta transakcije
        /// </summary>
        [Display(Name = "IBAN subjekta")]
        [Required(ErrorMessage = "Potrebno je napisati valjani IBAN")]
        [RegularExpression(@"^[A-Z]{2}\d{2}[A-Z0-9]{1,30}$", ErrorMessage = "Nevaljan format IBAN-a.")]
        public string SubjektIban { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja identifikator transakcije
        /// </summary>
        [Display(Name = "ID Transakcije")]
        [Required(ErrorMessage = "Potrebno je odabrati transakciju")]
        public int IdTransakcije { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja vrijednost transakcije
        /// </summary>
        [Display(Name = "Vrijednost")]
        [Required(ErrorMessage = "Potrebno je odabrati vrijednost")]
        public int Vrijednost { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja valutu transakcije
        /// </summary>
        [Display(Name = "Valuta")]
        [Required(ErrorMessage = "Potrebno je odabrati valutu")]
        public string Valuta { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja navigacijski objekt prema vrsti transakcije povezanoj s transakcijom
        /// </summary>
        public virtual VrstaTransakcije IdTransakcijeNavigation { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja navigacijski objekt prema projektnoj kartici povezanoj s transakcijom
        /// </summary>
        public virtual ProjektnaKartica SubjektIbanNavigation { get; set; }
    }
}
