using Microsoft.AspNetCore.Mvc;
using System;

namespace RPPP_WebApp.ViewModels
{
    /// <summary>
    /// Klasa koja predstavlja unos zapisa u log entry
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Dohvaca ili postavlja vrijeme unosa log-a
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja id log-a
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja naziv kontrolera
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja razinu zapisa
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja poruku zapisa
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja URL povezan s zapisom
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Dohvaca ili postavlja akciju povezanu s zapisom
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Stvara instancu LogEntry iz niza znakova
        /// </summary>
        /// <param name="text">Niz znakova koji sadrzi informacije o zapisu</param>
        /// <returns>Instanca LogEntry</returns>
        internal static LogEntry FromString(string text)
        {
            string[] arr = text.Split('|');
            LogEntry entry = new LogEntry();
            entry.Time = DateTime.ParseExact(arr[0], "yyyy-MM-dd HH:mm:ss.ffff", System.Globalization.CultureInfo.InvariantCulture);
            entry.Id = string.IsNullOrWhiteSpace(arr[1]) ? 0 : int.Parse(arr[1]);
            entry.Level = arr[2];
            entry.Controller = arr[3];
            entry.Message = arr[4];
            entry.Url = arr[5].Substring(5); //url: 
            entry.Action = arr[6].Substring(8); //action: 
            return entry;
        }
    }
}
