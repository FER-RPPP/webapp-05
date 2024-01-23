using RPPP_WebApp.Models;
using RPPP_WebApp.ModelsValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// Prikaz pomocnog modela s podacima o poslu
    /// </summary>
    public class PosaoPomocniViewModel
    {
        /// <summary>
        /// Predstavlja identifikacijski broj posla
        /// </summary>
        public int IdPosao { get; set; }

        /// <summary>
        /// Predstavlja Id vrste posla
        /// </summary>
        public int IdVrstaPosao { get; set; }

        /// <summary>
        /// Predstavlja opis posla
        /// </summary>
        [Display(Name = "Opis posla")]
        [Required(ErrorMessage = "Potrebno je unjeti opis posla")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Opis može imati samo slova.")]
        public string Opis { get; set; }

        /// <summary>
        /// Predstavlja predvideno vrijeme trajanja posla u danima
        /// </summary>
        [Display(Name = "Očekivano vrijeme trajanja u danima")]
        [Required(ErrorMessage = "Potrebno je unjeti očekivano vrijeme trajanja")]
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,3})?$", ErrorMessage = "Trajanje mora biti broj s maksimalno 3 decimale.")]
        public string PredVrTrajanjaDani { get; set; }

        /// <summary>
        /// Predstavlja ulogu suradnika u poslu
        /// </summary>
        [Display(Name = "Uloga")]
        [Required(ErrorMessage = "Potrebno je unjeti ulogu")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Uloga može imati samo slova.")]
        public string Uloga { get; set; }

        /// <summary>
        /// Predstavlja navigacijski objekt prema objektu VrstaPosla 
        /// </summary>
        public virtual VrstaPosla IdVrstaPosaoNavigation { get; set; }

        /// <summary>
        /// Predstavlja popis suradnika koji rade taj Posao
        /// </summary>
        public virtual ICollection<Suradnik> Suradnik { get; set; }

        /// <summary>
        /// Predstavlja naziv vrste posla za ovaj posao
        /// </summary>
        public string NazivPosao { get; set; }
    }
}