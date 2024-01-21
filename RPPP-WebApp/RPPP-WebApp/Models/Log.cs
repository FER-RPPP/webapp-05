#nullable disable

using System;

namespace RPPP_WebApp.Models
{
    /// <summary>
    /// Predstavlja zapis log-a
    /// </summary>
    public partial class Log
    {
        /// <summary>
        /// Dohvaca ili postavlja identifikator unosa log-a
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja naziv računala povezanog s unosom log
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja vremensku oznaku kada je zapis log stvoren
        /// </summary>
        public DateTime Logged { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja razinu
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja poruku 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja logger povezan s unosom log-a
        /// </summary>
        public string Logger { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja dodatna svojstva vezana uz unos log-a
        /// </summary>
        public string Properties { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja mjesto poziva gdje je zapis dnevnika log-a
        /// </summary>
        public string Callsite { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja informacije o iznimci povezanoj s unosom log-a
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja URL povezan s unosom log-a
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja upitni niz povezan s unosom log-a
        /// </summary>
        public string QueryString { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja radnju povezanu s unosom log-a
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja id traga povezan s log-om
        /// </summary>
        public string TraceId { get; set; }
    }
}
