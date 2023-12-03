using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace RPPP_WebApp.Controllers
{
    public class ZahtjevController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<ZahtjevController> logger;

        public ZahtjevController(RPPP05Context ctx, ILogger<ZahtjevController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
        }
        public IActionResult Index()
        {
            var zahtjevi = ctx.Zahtjev
                      .AsNoTracking()
                      .OrderBy(d => d.IdZahtjev)
                      .ToList();

            var model = new ZahtjevViewModel
            {
                zadatci = zahtjevi,
            };
            return View("Index", model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Zahtjev zahtjev)
        {
            logger.LogTrace(JsonSerializer.Serialize(zahtjev));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(zahtjev);
                    ctx.SaveChanges();
                    //logger.LogInformation(new EventId(1000), $"Zadatak {zadatak.IdZadatak} dodana.");

                    //TempData[Constants.Message] = $"Država {drzava.NazDrzave} dodana.";
                    //TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    //logger.LogError("Pogreška prilikom dodavanje nove države: {0}", exc.CompleteExceptionMessage());
                    //ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(zahtjev);
                }
            }
            else
            {
                return View(zahtjev);
            }
        }


        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var zahtjev = ctx.Zahtjev.AsNoTracking().Where(d => d.IdZahtjev == id).SingleOrDefault();
            if (zahtjev == null)
            {
                logger.LogWarning("Ne postoji zahtjev s oznakom: {0} ", id);
                return NotFound("Ne postoji zahtjev s oznakom: " + id);
            }
            else
            {
                //ViewBag.Page = page;
                //ViewBag.Sort = sort;
                //ViewBag.Ascending = ascending;
                return View(zahtjev);
            }
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            //za različite mogućnosti ažuriranja pogledati
            //attach, update, samo id, ...
            //https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud#update-the-edit-page

            try
            {
                Zahtjev zahtjev = await ctx.Zahtjev
                                  .Where(d => d.IdZahtjev == id)
                                  .FirstOrDefaultAsync();
                if (zahtjev == null)
                {
                    return NotFound("Neispravan id zahtjeva: " + id);
                }

                if (await TryUpdateModelAsync<Zahtjev>(zahtjev, "",
                    d => d.Opis, d => d.Prioritet, d => d.VrPocetak, d => d.VrKraj, d => d.VrKrajOcekivano
                ))
                {
                    ViewBag.Page = page;
                    ViewBag.Sort = sort;
                    ViewBag.Ascending = ascending;
                    try
                    {
                        await ctx.SaveChangesAsync();
                        ////TempData[Constants.Message] = "Država ažurirana.";
                        ////TempData[Constants.ErrorOccurred] = false;
                        return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                    }
                    catch (Exception exc)
                    {
                        //ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                        return View(zahtjev);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o državi nije moguće povezati s forme");
                    return View(zahtjev);
                }
            }
            catch (Exception exc)
            {
                //TempData[Constants.Message] = exc.CompleteExceptionMessage();
                //TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), id);
            }
        }
    }
}
