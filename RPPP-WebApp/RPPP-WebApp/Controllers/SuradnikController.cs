using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;
using RPPP_WebApp.Models;
using System.Reflection.Metadata.Ecma335;
using System.Linq;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Extensions.Selectors;

namespace RPPP_WebApp.Controllers
{
    public class SuradnikController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<SuradnikController> logger;
        private readonly AppSettings appSettings;

        public SuradnikController(RPPP05Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<SuradnikController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;
        }

        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;
            var query = ctx.Suradnik
                        .AsNoTracking();

            int count = query.Count();
            var pagingInfo = new PagingInfo
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

            var suradnici = query.Skip((page - 1) * pagesize)
                                  .Take(pagesize)
                                  .ToList();

            var kvalifikacija = query
                                .Skip((page - 1) * pagesize)
                                .Take(pagesize)
                                .Select(m => m.IdKvalifikacijaNavigation.NazivKvalifikacija)
                                .ToList();

            var listaposlova = suradnici
                               .Select(suradnik => string.Join(",", ctx.Posao
                                .Where(z => z.IdSuradnik.Contains(suradnik))
                                .Select(z => z.IdPosao)))
                                .ToList();

            var model = new SuradnikViewModel
            {
                suradnici = suradnici,
                kvalifikacija = kvalifikacija,
                PagingInfo = pagingInfo,
                poslovi = listaposlova,
            };

            return View(model);
        }

        private async Task PrepareDropDownLists()
        {
            var hr = await ctx.Kvalifikacija
                              .Where(d => d.IdKvalifikacija == 1)
                              .Select(d => new { d.NazivKvalifikacija, d.IdKvalifikacija })
                              .FirstOrDefaultAsync();
            var suradnici = await ctx.Kvalifikacija
                                    .Where(d => d.IdKvalifikacija != 1)
                                    .OrderBy(d => d.NazivKvalifikacija)
                                    .Select(d => new { d.NazivKvalifikacija , d.IdKvalifikacija})
                                    .ToListAsync();

            if (hr != null)
            {
                suradnici.Insert(0, hr);
            }
            ViewBag.SuradniciKvalifikacije = new SelectList(suradnici, nameof(hr.IdKvalifikacija), nameof(hr.NazivKvalifikacija));
            
            var hrv = await ctx.Partner
                                .Where(d => d.IdPartner == 1)
                                .Select(d => new { d.NazivPartner, d.IdPartner })
                                .FirstOrDefaultAsync();
            var partneri = await ctx.Partner
                                    .Where(d => d.IdPartner != 1)
                                    .OrderBy(d => d.NazivPartner)
                                    .Select(d => new {d.NazivPartner , d.IdPartner})
                                    .ToListAsync();

            if (hrv != null)
            {
                partneri.Insert(0 , hrv);
            }
            ViewBag.PartneriPopis = new SelectList(partneri, nameof(hrv.IdPartner), nameof(hrv.NazivPartner));
        }

        [HttpGet]

        public async Task<IActionResult> Create()
        { 
            await PrepareDropDownLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Suradnik suradnik)
        {
            logger.LogTrace(JsonSerializer.Serialize(suradnik));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(suradnik);
                    ctx.SaveChanges();
                    logger.LogInformation(new EventId(1000), $"Suradnik {suradnik.IdSuradnik} dodan,");

                    TempData[Constants.Message] = $"Suradnik {suradnik.IdSuradnik} dodan.";
                    TempData[Constants.ErrorOccurred] = false;

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exception)
                {
                    logger.LogError("Pogreška prilikom dodavanja novog suradnika: {0}", exception.Message);
                    ModelState.AddModelError(string.Empty, exception.Message);
                    await PrepareDropDownLists();
                    return View(suradnik);
                }
            } 
            else
            {
                await PrepareDropDownLists();
                return View(suradnik);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var suradnik = ctx.Suradnik
                                .AsNoTracking()
                                .Where(d => d.IdSuradnik == id)
                                .SingleOrDefault();
            if (suradnik == null)
            {
                logger.LogWarning("Ne postoji suradnik s oznakom: {0} ", id);
                return NotFound("Ne postoji suradnik s oznakom: " + id);
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                await PrepareDropDownLists();

                return View(suradnik);
            }    
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true, string opis = "opis")
        {
            try
            {
                Suradnik suradnik = await ctx.Suradnik
                                          .Where(d => d.IdSuradnik == id)
                                          .FirstOrDefaultAsync();
                if (suradnik == null)
                {
                    return NotFound("Neispravan id suradnika: " + id);
                }

                if (await TryUpdateModelAsync<Suradnik>(suradnik, 
                    "",
                    d => d.Oib,
                    d => d.Mobitel,
                    d => d.Ime,
                    d => d.Prezime,
                    d => d.Mail,
                    d => d.Stranka,
                    d => d.IdKvalifikacija,
                    d => d.IdPartner))
                {
                    ViewBag.Page = page;
                    ViewBag.Sort = sort;
                    ViewBag.Ascending = ascending;

                    try
                    {
                        await ctx.SaveChangesAsync();
                        TempData[Constants.Message] = "Suradnik " + id + " ažuriran.";
                        TempData[Constants.ErrorOccurred] = false;
                        return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                    } catch (Exception exception)
                    {
                        ModelState.AddModelError(string.Empty, exception.Message);
                        return View(suradnik);
                    }
                } else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o suradniku nije moguće povezati s forme");
                    return View(suradnik);
                }
            }
            catch (Exception exception)
            {
                TempData[Constants.Message] = exception.Message;
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), id);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Delete(string oib, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.OnajKojiBrisem = oib;
            var suradnik = ctx.Suradnik.Find(oib);
            if (suradnik != null)
            {
                try
                {
                    int id = suradnik.IdSuradnik;
                    ctx.Remove(suradnik);
                    ctx.SaveChanges();
                    logger.LogInformation($"Suradnik {id} uspješno obrisan");
                    TempData[Constants.Message] = $"Suradnik {id} uspješno obrisan";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja suradnika: " + exc.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja suradnika: " + exc.Message);
                }
            }
            else
            {
                logger.LogWarning("Ne postoji suradnik s oznakom: {0} ", oib);
                TempData[Constants.Message] = "Ne postoji suradnik s oznakom: " + oib;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }


        public async Task<IActionResult> Show(int id, int idPartner, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Show))
        {

            if (id == 0)
            {
                id = (await ctx.Suradnik.FirstOrDefaultAsync()).IdSuradnik;
            }

            var suradnik = await ctx.Suradnik
                                    .Where(d => d.IdSuradnik == id)
                                    .Select(d => new Suradnik
                                    {
                                        IdSuradnik = d.IdSuradnik,
                                        Oib = d.Oib,
                                        Mobitel = d.Mobitel,
                                        Ime = d.Ime,
                                        Prezime = d.Prezime,
                                        Mail = d.Mail,
                                        Stranka = d.Stranka,
                                        IdKvalifikacija = d.IdKvalifikacija,
                                        IdPartner = d.IdPartner
                                    })
                                    .FirstOrDefaultAsync();
            int pagesize = 10;
            var query = ctx.Suradnik
                     .AsNoTracking();

            int count = query.Count();
            var pagingInfo = new PagingInfo
            {
                CurrentPage = page,
                Sort = sort,
                Ascending = ascending,
                ItemsPerPage = pagesize,
                TotalItems = count
            };

            if (suradnik == null)
            {
                return NotFound($"Suradnik {id} ne postoji");
            } 
            else
            {
                string nazivKvalifikacije = await ctx.Kvalifikacija
                                                     .Where(d => d.IdKvalifikacija == suradnik.IdKvalifikacija)
                                                     .Select(d => d.NazivKvalifikacija)
                                                     .FirstOrDefaultAsync();

                List<Suradnik> svisuradnici = (await ctx.Suradnik.ToListAsync());
                int index = svisuradnici.FindIndex(d => d.IdSuradnik == suradnik.IdSuradnik);

                int idprethodnog = -1;
                int idsljedeceg = -1;

                if (index != 0)
                {
                    idprethodnog = svisuradnici[index - 1].IdSuradnik;
                }
                if (index != svisuradnici.Count - 1)
                {
                    idsljedeceg = svisuradnici[index + 1].IdSuradnik;
                }

                var poslovi = await ctx.Posao
                                      .Where(s => s.IdSuradnik.Contains(suradnik))
                                      .OrderBy(s => s.IdPosao)
                                      .Select(s => new Posao
                                      {
                                          IdPosao = s.IdPosao,
                                          IdVrstaPosao = s.IdVrstaPosao,
                                          Opis = s.Opis,
                                          PredVrTrajanjaDani = s.PredVrTrajanjaDani,
                                          Uloga = s.Uloga,
                                      })
                                      .ToListAsync();

                var vrstaposla = ctx.Posao.AsNoTracking()
                         .Where(d => suradnik.IdPosao.Contains(d))
                         .Select(m => m.IdVrstaPosaoNavigation.NazivPosao)
                         .ToList();

                var model = new PosaoViewModel
                {
                    poslovi = poslovi,
                    vrstaPosla = vrstaposla,
                    PagingInfo = pagingInfo,
                };


                var CIJELAPREDAJA = new SuradnikPosaoViewModel
                {
                    suradnik = suradnik,
                    Kvalifikacija = nazivKvalifikacije,
                    poslovi = model,
                    IdPrethSuradnik = idprethodnog,
                    IdSljedSuradnik = idsljedeceg,
                    PagingInfo = pagingInfo,
                };

                //await SetPreviousAndNext(position.Value, filter, sort, ascending);


                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;


                return View(viewName, CIJELAPREDAJA);

            }
        }
    }

}
