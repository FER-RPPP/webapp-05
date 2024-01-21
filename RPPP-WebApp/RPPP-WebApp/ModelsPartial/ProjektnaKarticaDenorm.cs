using Microsoft.AspNetCore.Mvc;
using System;

namespace RPPP_WebApp.Models
{
    /// <summary>
    /// Denormalizirani pogled projektnih kartica
    /// </summary>
    public class ProjektnaKarticaDenorm
    {
        /// <summary>
        /// Dohvaca ili postavlja IBAN subjekta
        /// </summary>
        public string subjektIBAN { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja saldo
        /// </summary>
        public int Saldo { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja valutu
        /// </summary>
        public string valuta { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja vrijeme otvaranja
        /// </summary>
        public DateTime vrijemeOtvaranja { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja identifikator projekta
        /// </summary>
        public int idProjekt { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja URL projektnih kartica
        /// </summary>
        public string KarticaUrl { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja IBAN primatelja
        /// </summary>
        public string primateljIBAN { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja opis transakcije
        /// </summary>
        public string opis { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja vrstu transakcije
        /// </summary>
        public string vrsta { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja identifikator transakcije
        /// </summary>
        public int idTransakcija { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja vrijednost transakcije
        /// </summary>
        public int vrijednost { get; set; }
    }
}
