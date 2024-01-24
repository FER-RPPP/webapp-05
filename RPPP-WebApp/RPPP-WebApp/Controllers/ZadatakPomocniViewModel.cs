using RPPP_WebApp.Models;
using RPPP_WebApp.ModelsValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// ViewModel za prikaz zadataka koji se koristi za Update master-detail forme.
    /// </summary>
    public class ZadatakPomocniViewModel
    {
        /// <summary>
        /// Dohvaća ili postavlja datum završetka rada na zadatku uz validaciju.
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Datum")]
        [DateGreaterThan(nameof(VrPoc), ErrorMessage = "Vrijeme kraja mora biti nakon vremena početka")]
        public DateTime? VrKraj { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja datum početka rada na zadatku uz validaciju.
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Datum")]
        [Required(ErrorMessage = "Potrebno je odabrati datum ")]
        public DateTime VrPoc { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja OIB nositelja zadatka uz validaciju.
        /// </summary>
        [Display(Name = "OIB nositelja")]
        [Required(ErrorMessage = "Potrebno je napisati OIB nositelja")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "OIB mora sadržavati točno 11 znamenki.")]
        [StringLength(11, ErrorMessage = "OIB mora imati točno 11 znakova.")]
        public string Oibnositelj { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja identifikator zadatka.
        /// </summary>
        public int IdZadatak { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja vrstu zadatka uz validaciju.
        /// </summary>
        [Required(ErrorMessage = "Potrebno je unijeti vrstu")]
        [Display(Name = "Vrsta")]
        [StringLength(20, ErrorMessage = "Vrsta ima maksimalno 20 znakova.")]
        public string Vrsta { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja očekivani datum završetka zadatka uz validaciju.
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Datum")]
        [Required(ErrorMessage = "Potrebno je odabrati datum ")]
        [DateGreaterThan(nameof(VrPoc), ErrorMessage = "Očekivano vrijeme kraja mora biti nakon vremena početka")]
        public DateTime VrKrajOcekivano { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja identifikator zahtjeva povezanog s zadatkom.
        /// </summary>
        public int IdZahtjev { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja identifikator statusa zadatka.
        /// </summary>
        [Display(Name = "Status")]
        [Required(ErrorMessage = "Potrebno je odabrati status")]
        public int IdStatus { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja objekt statusa povezanog s zadatkom uz pomoć druge tablice u bazi.
        /// </summary>
        public virtual StatusZadatka IdStatusNavigation { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja objekt zahtjeva povezanog s zadatkom uz pomoć druge tablice u bazi.
        /// </summary>
        public virtual Zahtjev IdZahtjevNavigation { get; set; }

        /// <summary>
        /// Dohvaća ili postavlja naziv statusa zadatka.
        /// </summary>
        public string NazivStatus { get; set; }
    }

}


