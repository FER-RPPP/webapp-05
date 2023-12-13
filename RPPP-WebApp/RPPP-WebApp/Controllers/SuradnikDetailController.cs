using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;
using RPPP_WebApp.Models;


namespace RPPP_WebApp.Controllers
{
    public class SuradnikDetailController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<SuradnikDetailController> logger;

        public SuradnikDetailController(RPPP05Context ctx, ILogger<SuradnikDetailController> logger)
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
                //return RedirectToAction(nameof(Index),new { page = pagingInfo.TotalPages, sort, ascending });
            }

            //query = query.ApplySort(sort, ascending);


            var suradnici = ctx.Suradnik
                      .AsNoTracking()
                      .Skip((page - 1) * pagesize)
                      .Take(pagesize)
                      .OrderBy(d => d.IdSuradnik)
                      .ToList();
            var kvalifikacije = ctx.Suradnik.AsNoTracking()
                         .Skip((page - 1) * pagesize)
                         .Take(pagesize)
                         .Select(m => m.IdKvalifikacijaNavigation.NazivKvalifikacija)
                         .ToList();

            var model = new SuradnikDetailViewModel
            {
                suradnici = suradnici,
                nazivKvalifikacije = kvalifikacije,
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
                                  .Select(d => new { d.NazivKvalifikacija, d.IdKvalifikacija })
                                  .ToListAsync();
            if (hr != null)
            {
                suradnici.Insert(0, hr);
            }
            ViewBag.SuradniciKvalifikacije = new SelectList(suradnici, nameof(hr.IdKvalifikacija), nameof(hr.NazivKvalifikacija));

            var hrv = await ctx.Partner
                                  .Where(d => d.IdPartner == 1)
                                  .Select(d => d.EmailPartner + " (id: " + d.IdPartner + ")")
                                  .FirstOrDefaultAsync();
            var partneri = await ctx.Partner
                                  .Where(d => d.IdPartner != 1)
                                  .OrderBy(d => d.EmailPartner)
                                  .Select(d => d.EmailPartner + " (id: " + d.IdPartner + ")")
                                  .ToListAsync();
            if (hrv != null)
            {
                partneri.Insert(0, hrv);
            }
            ViewBag.ParteriPopis = new SelectList(partneri, nameof(hrv));

        }

        [HttpGet]
        public async Task<IActionResult> CreateAsync()
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
                    logger.LogInformation(new EventId(1000), $"Suradnik {suradnik.IdSuradnik} dodan.");

                    TempData[Constants.Message] = $"Suradnik {suradnik.IdSuradnik} dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja novog suradnika: {0}", exc.Message);
                    ModelState.AddModelError(string.Empty, exc.Message);
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
            var suradnik = ctx.Suradnik.AsNoTracking().Where(d => d.IdSuradnik == id).SingleOrDefault();
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
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true, string email = "email")
        {
            //za različite mogućnosti ažuriranja pogledati
            //attach, update, samo id, ...
            //https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud#update-the-edit-page

            try
            {
                Suradnik suradnik = await ctx.Suradnik
                                  .Where(d => d.IdSuradnik == id)
                                  .FirstOrDefaultAsync();
                if (suradnik == null)
                {
                    return NotFound("Neispravan id suradnika: " + id);
                }

                if (await TryUpdateModelAsync<Suradnik>(suradnik, "",
                    d => d.Oib, d => d.IdKvalifikacija, d => d.Mobitel, d => d.Ime, d => d.Prezime, d => d.Mail, d => d.Stranka
                ))
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
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError(string.Empty, exc.Message);
                        return View(suradnik);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o suradniku nije moguće povezati s forme");
                    return View(suradnik);
                }
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = exc.Message;
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), id);
            }


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int IdSuradnik, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.OnajKojiBrisem = IdSuradnik;
            var suradnik = ctx.Suradnik.Find(IdSuradnik);
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
                logger.LogWarning("Ne postoji suradnik s oznakom: {0} ", IdSuradnik);
                TempData[Constants.Message] = "Ne postoji suradnik s oznakom: " + IdSuradnik;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }
    } 
 }