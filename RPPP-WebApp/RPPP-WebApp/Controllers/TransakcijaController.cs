using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using System.Text.Json;

namespace RPPP_WebApp.Controllers
{
    public class TransakcijaController : Controller
    {

        private readonly RPPP05Context ctx;
        private readonly ILogger<TransakcijaController> logger;
        private readonly AppSettings appSettings;

        public TransakcijaController(RPPP05Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<TransakcijaController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;
        }

        // GET: Transakcija
        [HttpGet]
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;

            var query = ctx.Transakcija
                           .AsNoTracking();

            int count = query.Count();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji nijedna transakcija");
                TempData[Constants.Message] = "Ne postoji niti jedna transakcija.";
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

            var transakcija = query
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToList();

            var model = new TransakcijaViewModel
            {
                Transakcija = transakcija,
                PagingInfo = pagingInfo
            };

            return View(model);
        }

        // GET: Transakcija/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: Transakcija/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Transakcija transakcija)
        {
            logger.LogTrace(JsonSerializer.Serialize(transakcija));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(transakcija);
                    ctx.SaveChanges();
                    logger.LogInformation(new EventId(1000), $"Transakcija {transakcija.IdTransakcije} dodana.");

                    TempData[Constants.Message] = $"Transakcija {transakcija.IdTransakcije} dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanje nove transakcije: {0}", exc.Message);
                    ModelState.AddModelError(string.Empty, exc.Message);
                    return View(transakcija);
                }
            }
            else
            {
                return View(transakcija);
            }
        }

        /*ZBOG NEKOG RAZLOGA ID JE JEDNAK PROJEKTID-u A NE IBANU SUBJEKTA*/
        // GET: ProjektnaKartica/Edit/5
        [HttpGet]
        public IActionResult Edit(string id, int page = 1, int sort = 1, bool ascending = true)
        {
            var transakcija = ctx.Transakcija.AsNoTracking().Where(d => d.PrimateljIban == id).SingleOrDefault();
            if (transakcija == null)
            {
                logger.LogWarning("Ne postoji transakcija za IBAN: {0} ", id);
                return NotFound("Ne postoji transakcija za IBAN:  " + id);
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(transakcija);
            }
        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, int page = 1, int sort = 1, bool ascending = true)
        {

            try
            {
                Transakcija transakcija = await ctx.Transakcija
                                  .Where(d => d.PrimateljIban == id)
                                  .FirstOrDefaultAsync();
                if (transakcija == null)
                {
                    return NotFound("Neispravan IBAN: " + id);
                }

                if (await TryUpdateModelAsync<Transakcija>(transakcija, "",
                    d => d.opis, d => d.Vrsta,d => d.IdTransakcije
                ))
                {
                    ViewBag.Page = page;
                    ViewBag.Sort = sort;
                    ViewBag.Ascending = ascending;
                    try
                    {
                        await ctx.SaveChangesAsync();
                        TempData[Constants.Message] = "Transakcija ažurirana.";
                        TempData[Constants.ErrorOccurred] = false;
                        return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError(string.Empty, exc.Message);
                        return View(transakcija);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o transakciji nije moguće povezati s forme");
                    return View(transakcija);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string primateljIBAN, int page = 1, int sort = 1, bool ascending = true)
        {
            var transakcija = ctx.Transakcija.Find(primateljIBAN);

            if (transakcija != null)
            {
                try
                {
                    string naziv = transakcija.PrimateljIban;
                    ctx.Remove(transakcija);
                    ctx.SaveChanges();
                    logger.LogInformation($"Transakcija {naziv} uspješno obrisana");
                    TempData[Constants.Message] = $"Transakcija {naziv} uspješno obrisana";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja transakcije: " + exc.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja transakcije: " + exc.Message);
                }
            }
            else
            {
                logger.LogWarning("Ne postoji transakcija IBAN: {0} ", transakcija);
                TempData[Constants.Message] = "Ne postoji transakcija za IBAN: " + transakcija;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }
    }
}
