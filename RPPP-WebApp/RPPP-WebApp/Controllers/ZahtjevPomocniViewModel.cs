using RPPP_WebApp.Models;
using RPPP_WebApp.ModelsValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// ViewModel za prikaz zahtjeva koji se koristi za Update master-detail forme
    /// </summary>
    public class ZahtjevPomocniViewModel
    {
        /// <summary>
        /// Dohvaća ili postavlja opis zahtjeva.
        /// </summary>
        [Display(Name = "Opis zahtjeva")]
        [Required(ErrorMessage = "Potrebno je napisati opis zahtjeva")]
        [StringLength(100, ErrorMessage = "Opis ima maksimalno 100 znakova.")]
        public string Opis { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja prioritet zahtjeva.
        /// </summary>
        [Required(ErrorMessage = "Potrebno je unijeti prioritet")]
        [Display(Name = "Prioritet")]
        [StringLength(20, ErrorMessage = "Prioritet ima maksimalno 20 znakova.")]
        public string Prioritet { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja identifikator zahtjeva.
        /// </summary>
        public int IdZahtjev { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja datum kraja izvršenja zahtjeva.
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Datum")]
        [DateGreaterThan(nameof(VrPocetak), ErrorMessage = "Vrijeme kraja mora biti nakon vremena početka")]
        public DateTime? VrKraj { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja očekivani datum kraja izvršenja zahtjeva.
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Datum")]
        [Required(ErrorMessage = "Potrebno je odabrati datum ")]
        [DateGreaterThan(nameof(VrPocetak), ErrorMessage = "Očekivano vrijeme kraja mora biti nakon vremena početka")]
        public DateTime VrKrajOcekivano { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja datum početka izvršenja zahtjeva.
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Datum")]
        [Required(ErrorMessage = "Potrebno je odabrati datum ")]
        public DateTime VrPocetak { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja identifikator vrste zahtjeva.
        /// </summary>
        [Display(Name = "Vrsta")]
        [Required(ErrorMessage = "Potrebno je odabrati vrstu")]
        public int IdVrsta { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja identifikator projekta.
        /// </summary>
        [Display(Name = "IdProjekt")]
        [Required(ErrorMessage = "Potrebno je odabrati projekt")]
        public int IdProjekt { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja projektni objekt povezan s ovim zahtjevom uz pomoć druge tablice iz baze.
        /// </summary>
        public virtual Projekt IdProjektNavigation { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja vrstu zahtjeva povezanu s ovim zahtjevom uz pomoć druge tablice iz baze.
        /// </summary>
        public virtual VrstaZahtjeva IdVrstaNavigation { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja kolekciju zadataka povezanu s ovim zahtjevom.
        /// </summary>
        public virtual ICollection<Zadatak> Zadatak { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja naziv vrste zahtjeva.
        /// </summary>
        public string NazivVrsta { get; set; }
    }
}
