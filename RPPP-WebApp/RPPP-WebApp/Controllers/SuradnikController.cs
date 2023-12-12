using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;
using RPPP_WebApp.Models;
using System.Reflection.Metadata.Ecma335;

namespace RPPP_WebApp.Controllers
{
    public class SuradnikController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<SuradnikController> logger;

        public SuradnikController(RPPP05Context ctx, ILogger<SuradnikController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
        }

        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
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

            if (page < 1 || page > pagingInfo.TotalPages)
            {
                return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
            }

            var suradnici = ctx.Suradnik
                            .AsNoTracking()
                            .Skip((page - 1) * pagesize)
                            .Take(pagesize)
                            .OrderBy(d => d.IdSuradnik)
                            .ToList();

            var kvalifikacija = ctx.Suradnik
                                .AsNoTracking()
                                .Skip((page - 1) * pagesize)
                                .Take(pagesize)
                                .Select(m => m.IdKvalifikacijaNavigation.NazivKvalifikacija)
                                .ToList();

            var model = new SuradnikViewModel
            {
                suradnici = suradnici,
                kvalifikacija = kvalifikacija,
                PagingInfo = pagingInfo,
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
            
            var hrv = await ctx.Projekt
                                .Where(d => d.IdProjekt == 1)
                                .Select(d => new { d.Naziv, d.IdProjekt })
                                .FirstOrDefaultAsync();
            var projekti = await ctx.Projekt
                                    .Where(d => d.IdProjekt == 1)
                                    .OrderBy(d => d.Naziv)
                                    .Select(d => new {d.Naziv , d.IdProjekt})
                                    .ToListAsync();

            if (hrv != null)
            {
                projekti.Insert(0 , hrv);
            }
            ViewBag.ProjektiPopis = new SelectList(projekti, nameof(hrv.IdProjekt), nameof(hrv.Naziv));
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
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
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

        public IActionResult Delete(int idSuradnik, int page = 1, int sort = 1, bool ascending = true)
        {
            var suradnik = ctx.Suradnik.Find(idSuradnik);
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
                catch (Exception exception)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja suradnika: " + exception.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja suradnika: " + exception.Message);
                }
            }
            else
            {
                logger.LogWarning("Ne postoji suradnik s oznakom: {0} ", idSuradnik);
                TempData[Constants.Message] = "Ne postoji suradnik s oznakom: " + idSuradnik;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }

        public int GetIdPartner()
        {
            return IdPartner;
        }

        public async Task<IActionResult> Show(int id, int idPartner, int page = 1, int sort = 1, bool ascending = true)
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
                                      .Where(s => s == suradnik.IdPosao)
                                      .OrderBy(s => s.IdPosao)
                                      .Select(s => new Posao
                                      {
                                          IdPosao = s.IdPosao,
                                          IdVrstaPosao = s.IdVrstaPosao
                                          
                                      })
                                      .ToListAsync();

                var vrstaposla = ctx.Posao.AsNoTracking()
                         .Where(d => d == suradnik.IdPosao)
                         .Select(m => m.IdVrstaPosaoNavigation.na)
                         .ToList();

                var model = new PosaoViewModel
                {
                    ///
                };


                var CIJELAPREDAJA = new SuradnikPosaoViewModel
                {
                    ///
                    IdPrethZahtjev = idprethodnog,
                    IdSljedZahtjev = idsljedeceg,
                    

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
