using RPPP_WebApp.Models;
using RPPP_WebApp.ModelsValidation;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.ViewModels.PD_viewModels
{
    /// <summary>
    /// Pomocni view model za projekt
    /// </summary>
    public class ProjektPomocni
    {
        /// <summary>
        /// Instanca projekta
        /// </summary>
        public ProjektPomocni()
        {
            Dokument = new HashSet<Dokument>();
            ProjektnaKartica = new HashSet<ProjektnaKartica>();
            Uloga = new HashSet<Uloga>();
            Zahtjev = new HashSet<Zahtjev>();
            IdPartner = new HashSet<Partner>();
        }
        /// <summary>
        /// ID projekta
        /// </summary>
        public int IdProjekt { get; set; }

        /// <summary>
        /// Vrijeme početka projekta
        /// </summary>
        [Display(Name = "Datum početka projekta")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Potrebno je napisati ili izabrati datum")]
        public DateTime VrPocetak { get; set; }

        /// <summary>
        /// Vrijeme kraja projekta
        /// </summary>
        [Display(Name = "Datum kraja projekta")]
        [DataType(DataType.Date)]
        [DateGreaterThan(nameof(VrPocetak), ErrorMessage = "Vrijeme kraja mora biti nakon vremena početka")]
        public DateTime? VrKraj { get; set; }
        /// <summary>
        /// Opis projekta
        /// </summary>
        [Display(Name = "Opis projekta")]
        [Required(ErrorMessage = "Potrebno je napisati opis projekta")]
        public string Opis { get; set; }
        /// <summary>
        /// Naziv projekta
        /// </summary>
        [Display(Name = "Naziv projekta")]
        [Required(ErrorMessage = "Potrebno je napisati naziv projekta")]
        public string Naziv { get; set; }

        /// <summary>
        /// ID vrste projekta
        /// </summary>
        [Display(Name = "Vrsta projekta")]
        [Required(ErrorMessage = "Potrebno je odabrati vrstu projekta")]
        public int IdTip { get; set; }

        public virtual TipProjekta IdTipNavigation { get; set; }
        public virtual ICollection<Dokument> Dokument { get; set; }
        public virtual ICollection<ProjektnaKartica> ProjektnaKartica { get; set; }
        public virtual ICollection<Uloga> Uloga { get; set; }
        public virtual ICollection<Zahtjev> Zahtjev { get; set; }
        public virtual ICollection<Partner> IdPartner { get; set; }

        /// <summary>
        /// Naziv vrste projekta
        /// </summary>
        public string NazivTip { get; set; }
        /// <summary>
        /// Pomoćna kolekcija dokumenata
        /// </summary>
        public IEnumerable<DokPomViewModel> dokumenti { get; set;}
    }
}
