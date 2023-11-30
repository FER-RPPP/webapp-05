using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Controllers
{
    public class ZadatakController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<ZadatakController> logger;

        public ZadatakController(RPPP05Context ctx, ILogger<ZadatakController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
        }
        public IActionResult Index()
        {
            var zadatci = ctx.Zadatak
                      .AsNoTracking()
                      .OrderBy(d => d.IdZadatak)
                      .ToList();
      return View("Index", zadatci);
      //return View();
        }
    }
}
