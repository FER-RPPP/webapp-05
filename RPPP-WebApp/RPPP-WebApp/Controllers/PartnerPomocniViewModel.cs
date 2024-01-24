using RPPP_WebApp.Models;
using RPPP_WebApp.ModelsValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// Prikaz partnera. Koristi se kod uredivanja u MD formi
    /// </summary>
    public class PartnerPomocniViewModel
    {
        /// <summary>
        /// Dohvaća ili postavlja Identifikator (Id) partnera
        /// </summary>
        public int IdPartner { get; set; }

        /// <summary>
        /// Dohvaca ili posatavlja OIB partnera
        /// </summary>
        [Display(Name = "OIB")]
        [Required(ErrorMessage = "Potrebno je unjeti OIB")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "OIB mora sadržavati točno 11 znamenki.")]
        [StringLength(11, ErrorMessage = "OIB mora imati točno 11 znakova.")]
        public string Oib { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja adresu partnera
        /// </summary>
        [Display(Name = "Adresa")]
        [Required(ErrorMessage = "Potrebno je unjeti adresu")]
        [RegularExpression(@"^[a-zA-ZČčĆćŽžŠšĐđ]+(\s[a-zA-ZČčĆćŽžŠšĐđ]+)*\s\d+[A-Za-zČčĆćŽžŠšĐđ]?$", ErrorMessage = "Adresa treba početi riječima i sadržavati kućanski broj bez navođenja grada.")]
        public string AdresaPartner { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja IBAN partnera
        /// </summary>
        [Display(Name = "IBAN")]
        [Required(ErrorMessage = "Potrebno je unjeti IBAN")]
        [RegularExpression(@"^[A-Za-z]{2}\d{19}$", ErrorMessage = "IBAN mora početi s 2 slova i zatim imati 19 brojki.")]
        [StringLength(21, ErrorMessage = "IBAN mora imati točno 21 znak.")]
        public string Ibanpartner { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja Email partnera
        /// </summary>
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Potrebno je unjeti Email adresu.")]
        [EmailAddress(ErrorMessage = "Email adresa nije validna.")]
        [RegularExpression(@"^[a-zA-Z0-9ČčĆćŽžŠšĐđ._%+-]+[._-]?[a-zA-Z0-9ČčĆćŽžŠšĐđ._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email adresa nije validna.")]
        public string EmailPartner { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja naziv partnera
        /// </summary>
        public string NazivPartner { get; set; }

        /// <summary>
        /// Dohvaca ili posatavlja Id tipa (vrste partnera)
        /// </summary>
        public int IdTipPartnera { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja tip partnera povezan s oim partnerom uz pomoć druge tablice u bazi podataka
        /// </summary>
        public virtual TipPartnera IdTipPartneraNavigation { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja klekciju suradnika koji su povezani s odredenim partnerom
        /// </summary>
        public virtual ICollection<Suradnik> Suradnik { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja kolekciju Idprojekata na kojima je odredeni partner
        /// </summary>
        public virtual ICollection<Projekt> IdProjekt { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja projektni objekt povezan s ovim partnerom uz pomoc druge tablice u bazi podataka
        /// </summary>
        public virtual Projekt IdProjektNavigation { get; set; }
        
        /// <summary>
        /// Dohvaca ili postavlja tip partnera
        /// </summary>
        public string TipPartnera1 { get; set; }

    }
}
