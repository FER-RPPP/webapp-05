#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Models
{
    /// <summary>
    /// Predstavlja projektnu karticu
    /// </summary>
    public partial class ProjektnaKartica
    {
        /// <summary>
        /// Inicijalizira novu instancu ProjektnaKartica/>.
        /// </summary>
        public ProjektnaKartica()
        {
            Transakcija = new HashSet<Transakcija>();
        }

        /// <summary>
        /// Dohvaca ili postavlja saldo projektna kartice
        /// </summary>
        public double Saldo { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja IBAN subjekta projektna kartice
        /// </summary>
        [Display(Name = "IBAN subjekta")]
        [Required(ErrorMessage = "Potrebno je napisati valjani IBAN")]
        [RegularExpression(@"^[A-Z]{2}\d{2}[A-Z0-9]{1,30}$", ErrorMessage = "Nevaljan format IBAN-a.")]
        public string SubjektIban { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja valutu projektna kartice
        /// </summary>
        [Display(Name = "Valuta")]
        [Required(ErrorMessage = "Potrebno je odabrati valutu")]
        public string Valuta { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja datum otvaranja projektna kartice
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Datum")]
        [Required(ErrorMessage = "Potrebno je odabrati datum")]
        public DateTime VrijemeOtvaranja { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja identifikator projekta povezanog s projektnom karticom
        /// </summary>
        [Display(Name = "Projekt")]
        [Required(ErrorMessage = "Potrebno je odabrati projekt/projektId postojećeg projekta")]
        public int IdProjekt { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja navigacijski objekt prema projektu povezanom s projektnom karticom
        /// </summary>
        public virtual Projekt IdProjektNavigation { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja kolekciju transakcija povezanih s projektnom karticom
        /// </summary>
        public virtual ICollection<Transakcija> Transakcija { get; set; }
    }
}
