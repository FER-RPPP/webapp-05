using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using System.Text.Json;

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
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        // POST: ProjektnaKartica/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProjektnaKartica proj_kartica)
        {
            logger.LogTrace(JsonSerializer.Serialize(proj_kartica));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(proj_kartica);
                    ctx.SaveChanges();
                    logger.LogInformation(new EventId(1000), $"Projektna kartica za projekt {proj_kartica.IdProjekt} dodana.");

                    TempData[Constants.Message] = $"Projektna kartica za projekt {proj_kartica.IdProjekt} dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanje nove projektne kartice: {0}", exc.Message);
                    ModelState.AddModelError(string.Empty, exc.Message);
                    return View(proj_kartica);
                }
            }
            else
            {
                return View(proj_kartica);
            }
        }

        /*ZBOG NEKOG RAZLOGA ID JE JEDNAK PROJEKTID-u A NE IBANU SUBJEKTA*/
        // GET: ProjektnaKartica/Edit/5
        [HttpGet]
        public IActionResult Edit(string id, int page = 1, int sort = 1, bool ascending = true)
        {
            var kartica = ctx.ProjektnaKartica.AsNoTracking().Where(d => d.SubjektIban == id).SingleOrDefault();
            if (kartica == null)
            {
                logger.LogWarning("Ne postoji projektna kartica za IBAN: {0} ", id);
                return NotFound("Ne postoji projektna kartica za IBAN:  " + id);
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(kartica);
            }
        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, int page = 1, int sort = 1, bool ascending = true)
        {

            try
            {
                ProjektnaKartica kartica = await ctx.ProjektnaKartica
                                  .Where(d => d.SubjektIban == id)
                                  .FirstOrDefaultAsync();
                if (kartica == null)
                {
                    return NotFound("Neispravan IBAN: " + id);
                }

                if (await TryUpdateModelAsync<ProjektnaKartica>(kartica, "",
                    d => d.Saldo, d => d.Valuta
                ))
                {
                    ViewBag.Page = page;
                    ViewBag.Sort = sort;
                    ViewBag.Ascending = ascending;
                    try
                    {
                        await ctx.SaveChangesAsync();
                        TempData[Constants.Message] = "Projektna kartica ažurirana.";
                        TempData[Constants.ErrorOccurred] = false;
                        return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError(string.Empty, exc.Message);
                        return View(kartica);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o projektnoj kartici nije moguće povezati s forme");
                    return View(kartica);
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
        public IActionResult Delete(string subjektIBAN, int page = 1, int sort = 1, bool ascending = true)
        {
            var kartica = ctx.ProjektnaKartica.Find(subjektIBAN);
            
            if (kartica != null)
            {
                try
                {
                    string naziv = kartica.SubjektIban;
                    ctx.Remove(kartica);
                    ctx.SaveChanges();
                    logger.LogInformation($"Projektna kartica projekta {naziv} uspješno obrisana");
                    TempData[Constants.Message] = $"Projektna kartica projekta {naziv} uspješno obrisana";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja projektne kartice: " + exc.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja projektne kartice: " + exc.Message);
                }
            }
            else
            {
                logger.LogWarning("Ne postoji projektna kartica IBAN: {0} ", kartica);
                TempData[Constants.Message] = "Ne postoji projektna kartica za IBAN: " + kartica;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }
    }
}
