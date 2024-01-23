
using RPPP_WebApp.Controllers;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{

	/// <summary>
	/// Predstavlja model s podacima o suradniku i njegovim poslovima
	/// </summary>
	public class MDSuradniciViewModel
	{
		/// <summary>
		/// Predstavlja suradnika
		/// </summary>
		public Suradnik suradnik { get; set; }

		/// <summary>
		/// Predstavlja kvalifikaciju suradnika
		/// </summary>
		public string kvalifikacija { get; set; }

        /// <summary>
        /// Predstavlja identifikacijski broj prethodnog suradnika
        /// </summary>
        public int? IdPrethSuradnik { get; set; }

        /// <summary>
        /// Predstavlja identifikacijski broj sljedeceg suradnika
        /// </summary>
        public int? IdSljedSuradnik { get; set; }

        /// <summary>
        /// Predstavlja informacije o stranicenju
        /// </summary>
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// Predstavlja popis poslova koje suradnik radi
        /// </summary>
        public IEnumerable<PosaoPomocniViewModel> Poslovi { get; set; }

		/// <summary>
		/// Inicijalizacija nove isntance klase MDSuradniciViewModel
		/// </summary>
		public MDSuradniciViewModel()
		{
			this.Poslovi = new List<PosaoPomocniViewModel>();
		}

	}
}