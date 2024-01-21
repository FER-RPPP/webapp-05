using RPPP_WebApp.Models;
using RPPP_WebApp.ModelsValidation;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.ViewModels.PD_viewModels
{
    public class ProjektPomocni
    {

        public ProjektPomocni()
        {
            Dokument = new HashSet<Dokument>();
            ProjektnaKartica = new HashSet<ProjektnaKartica>();
            Uloga = new HashSet<Uloga>();
            Zahtjev = new HashSet<Zahtjev>();
            IdPartner = new HashSet<Partner>();
        }

        public int IdProjekt { get; set; }


        [Display(Name = "Datum početka projekta")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Potrebno je napisati ili izabrati datum")]
        public DateTime VrPocetak { get; set; }


        [Display(Name = "Datum kraja projekta")]
        [DataType(DataType.Date)]
        [DateGreaterThan(nameof(VrPocetak), ErrorMessage = "Vrijeme kraja mora biti nakon vremena početka")]
        public DateTime? VrKraj { get; set; }

        [Display(Name = "Opis projekta")]
        [Required(ErrorMessage = "Potrebno je napisati opis projekta")]
        public string Opis { get; set; }

        [Display(Name = "Naziv projekta")]
        [Required(ErrorMessage = "Potrebno je napisati naziv projekta")]
        public string Naziv { get; set; }


        [Display(Name = "Vrsta projekta")]
        [Required(ErrorMessage = "Potrebno je odabrati vrstu projekta")]
        public int IdTip { get; set; }

        public virtual TipProjekta IdTipNavigation { get; set; }
        public virtual ICollection<Dokument> Dokument { get; set; }
        public virtual ICollection<ProjektnaKartica> ProjektnaKartica { get; set; }
        public virtual ICollection<Uloga> Uloga { get; set; }
        public virtual ICollection<Zahtjev> Zahtjev { get; set; }

        public virtual ICollection<Partner> IdPartner { get; set; }


        public string NazivTip { get; set; }
    }
}
