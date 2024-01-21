using Microsoft.AspNetCore.Mvc;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// Klasa koja nasljeduje klasu kontroler u MVC aplikaciji koja dohvaca pocetnu stranicu web-aplikacije
    /// </summary>
  public class HomeController : Controller
  {
        /// <summary>
        /// Funkcija koja dohvaca prikaz pocetne stranice
        /// </summary>
        /// <returns>Pocetna stranica</returns>
    public IActionResult Index()
    {
      return View();
    }
  }
}
