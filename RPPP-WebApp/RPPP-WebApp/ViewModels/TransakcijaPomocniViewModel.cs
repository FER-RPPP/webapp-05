using Microsoft.AspNetCore.Mvc;

namespace RPPP_WebApp.ViewModels
{
    /// <summary>
    /// ViewModel klasa za pomoc pri prikazu transakcija
    /// </summary>
    public class TransakcijaPomocniViewModel : Controller
    {
        /// <summary>
        /// Akcija koja prikazuje stranicu za transakcije
        /// </summary>
        /// <returns>View rezultat za stranicu</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
