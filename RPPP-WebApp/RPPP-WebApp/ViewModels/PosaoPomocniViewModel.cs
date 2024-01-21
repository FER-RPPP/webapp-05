using RPPP_WebApp.Models;
using RPPP_WebApp.ModelsValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Controllers
{
    public class PosaoPomocniViewModel
    {
        public int IdPosao { get; set; }
        public int IdVrstaPosao { get; set; }

        [Display(Name = "Opis posla")]
        [Required(ErrorMessage = "Potrebno je unjeti opis posla")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Opis može imati samo slova.")]
        public string Opis { get; set; }

        [Display(Name = "Očekivano vrijeme trajanja u danima")]
        [Required(ErrorMessage = "Potrebno je unjeti očekivano vrijeme trajanja")]
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,3})?$", ErrorMessage = "Trajanje mora biti broj s maksimalno 3 decimale.")]
        public string PredVrTrajanjaDani { get; set; }

        [Display(Name = "Uloga")]
        [Required(ErrorMessage = "Potrebno je unjeti ulogu")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Uloga može imati samo slova.")]
        public string Uloga { get; set; }

        public virtual VrstaPosla IdVrstaPosaoNavigation { get; set; }

        public virtual ICollection<Suradnik> Suradnik { get; set; }

        public string NazivPosao { get; set; }
    }
}