
using RPPP_WebApp.Controllers;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
	public class MDSuradniciViewModel
	{
		public Suradnik suradnik { get; set; }
		public string kvalifikacija { get; set; }
		public int? IdPrethSuradnik { get; set; }
		public int? IdSljedSuradnik { get; set; }

		public PagingInfo PagingInfo { get; set; }

		public IEnumerable<PosaoPomocniViewModel> Poslovi { get; set; }

		public MDSuradniciViewModel()
		{
			this.Poslovi = new List<PosaoPomocniViewModel>();
		}

	}
}