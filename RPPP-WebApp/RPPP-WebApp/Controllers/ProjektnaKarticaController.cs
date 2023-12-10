using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MVC;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;

namespace RPPP_WebApp.Controllers
{
    public class ProjektnaKarticaController : Controller
    {

        private readonly RPPP05Context ctx;
        private readonly ILogger<ProjektnaKarticaController> logger;
        private readonly AppSettings appSettings;

        public ProjektnaKarticaController(RPPP05Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<ProjektnaKarticaController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;
        }

        // GET: ProjektnaKartica
        [HttpGet]
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;

            var query = ctx.ProjektnaKartica
                           .AsNoTracking();

            int count = query.Count();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji nijedna projektna kartica");
                TempData[Constants.Message] = "Ne postoji niti jedna projektna kartica.";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Create));
            }

            var pagingInfo = new Controllers.PagingInfo
            {
                CurrentPage = page,
                Sort = sort,
                Ascending = ascending,
                ItemsPerPage = pagesize,
                TotalItems = count
            };
            if (page < 1 || page > pagingInfo.TotalPages)
            {
                return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
            }

            //query = query.ApplySort(sort, ascending);

            var projektnaKartica = query
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToList();

            var model = new ProjektnaKarticaViewModel
            {
                ProjektnaKartica = projektnaKartica,
                PagingInfo = pagingInfo 
            };

            return View(model);
        }

        // GET: ProjektnaKartica/Create
        public ActionResult Create()
        {
            return View();
        }










        // POST: ProjektnaKartica/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProjektnaKartica/Edit/5
         [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true, string opis ="opis")
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
                    d => d.Opis, d => d.Prioritet, d => d.VrPocetak, d => d.VrKraj, d => d.VrKrajOcekivano, d => d.IdProjekt, d => d.IdVrsta
                ))
                {
                    ViewBag.Page = page;
                    ViewBag.Sort = sort;
                    ViewBag.Ascending = ascending;
                    try
                    {
                        await ctx.SaveChangesAsync();
                        TempData[Constants.Message] = "Zahtjev " + id + " ažuriran.";
                        TempData[Constants.ErrorOccurred] = false;
                        return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError(string.Empty, exc.Message);
                        return View(zahtjev);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o zahtjevu nije moguće povezati s forme");
                    return View(zahtjev);
                }
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = exc.Message;
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), id);
            }
        }


        // GET: ProjektnaKartica/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int IdZahtjev, int page = 1, int sort = 1, bool ascending = true)
        {
            var zahtjev = ctx.Zahtjev.Find(IdZahtjev);
            if (zahtjev != null)
            {
                try
                {
                    int id = zahtjev.IdZahtjev;
                    ctx.Remove(zahtjev);
                    ctx.SaveChanges();
                    logger.LogInformation($"Zahtjev {id} uspješno obrisana");
                    TempData[Constants.Message] = $"Zahtjev {id} uspješno obrisana";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja zahtjeva: " + exc.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja zahtjeva: " + exc.Message);
                }
            }
            else
            {
                logger.LogWarning("Ne postoji zahtjev s oznakom: {0} ", IdZahtjev);
                TempData[Constants.Message] = "Ne postoji zahtjev s oznakom: " + IdZahtjev;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }
    }
}
