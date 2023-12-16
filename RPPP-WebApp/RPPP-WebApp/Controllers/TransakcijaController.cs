using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using RPPP_WebApp.Extensions.Selectors;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

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

            query = query.ApplySort(sort, ascending);

            var transakcija = query
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToList();

            var vrste = query
                         .Skip((page - 1) * pagesize)
                         .Take(pagesize)
                         .Select(m => m.IdTransakcijeNavigation.NazivTransakcije)
                         .ToList();

            var model = new TransakcijaViewModel
            {
                Transakcija = transakcija,
                PagingInfo = pagingInfo
            };

            return View(model);
        }

        private async Task PrepareDropDownLists()
        {
            //TODO: napisati funkciju

            var hr = await ctx.VrstaTransakcije
                              .Where(d => d.IdTransakcije == 1)
                              .Select(d => new { d.NazivTransakcije, d.IdTransakcije })
                              .FirstOrDefaultAsync();
            var transakcije = await ctx.VrstaTransakcije
                                  .Where(d => d.IdTransakcije != 1)
                                  .OrderBy(d => d.NazivTransakcije)
                                  .Select(d => new { d.NazivTransakcije, d.IdTransakcije })
                                  .ToListAsync();
            if (hr != null)
            {
                transakcije.Insert(0, hr);
            }
            ViewBag.TransakcijeId = new SelectList(transakcije, nameof(hr.IdTransakcije), nameof(hr.NazivTransakcije));

            var hrv = await ctx.ProjektnaKartica
                                  .Where(d => d.SubjektIban == @"^hr\d{19}$")
                                  .Select(d => new { v = d.SubjektIban + " (id: " + d.IdProjekt + ")", d.Valuta })
                                  .FirstOrDefaultAsync();
            var kartice = await ctx.ProjektnaKartica
                                  .Where(d => d.SubjektIban != @"^hr\d{19}$")
                                  .OrderBy(d => d.SubjektIban)
                                  .Select(d => new { v = d.SubjektIban + " (id: " + d.IdProjekt + ")", d.Valuta })
                                  .ToListAsync();
            if (hrv != null)
            {
                kartice.Insert(0, hrv);
            }
            ViewBag.TransakcijePopis = new SelectList(kartice, nameof(hrv.Valuta), nameof(hrv.v));
        }



        // GET: Transakcija/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropDownLists();
            return View();
        }


        // POST: Transakcija/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transakcija transakcija)
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
                    await PrepareDropDownLists();
                    return View(transakcija);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(transakcija);
            }
        }

        
        // GET: ProjektnaKartica/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id, int page = 1, int sort = 1, bool ascending = true)
        {
            var transakcija = ctx.Transakcija.AsNoTracking().Where(d => d.SubjektIban == id).SingleOrDefault();
            if (transakcija == null)
            {
                logger.LogWarning("Ne postoji transakcija : {0} ", id);
                return NotFound("Ne postoji transakcija:  " + id);
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                await PrepareDropDownLists();
                return View(transakcija);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, int page = 1, int sort = 1, bool ascending = true, string opis = "opis")
        {

            try
            {
                Transakcija transakcija = await ctx.Transakcija
                                  .Where(d => d.SubjektIban == id)
                                  .FirstOrDefaultAsync();
                if (transakcija == null)
                {
                    return NotFound("Neispravan idTransakcije: " + id);
                }

                if (await TryUpdateModelAsync<Transakcija>(transakcija, "",
                    d => d.Opis, d => d.Vrsta,d => d.IdTransakcije,d =>d.Vrijednost, d=>d.Valuta
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
