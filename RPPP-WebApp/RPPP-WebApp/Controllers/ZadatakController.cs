using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;
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
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {

            int pagesize = 100;
            var query = ctx.Zadatak
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
                //return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
                return RedirectToAction(nameof(Index),new { page = pagingInfo.TotalPages, sort, ascending });
            }




            var zadatci = ctx.Zadatak
                      .AsNoTracking()
                      .Skip((page - 1) * pagesize)
                      .Take(pagesize)
                      .OrderBy(d => d.IdZadatak)
                      .ToList();
            var statusi = ctx.Zadatak.AsNoTracking()
                         .Skip((page - 1) * pagesize)
                         .Take(pagesize)
                         .Select(m =>  m.IdStatusNavigation.NazivStatus)
                         .ToList();

            var model = new ZadatakViewModel
            {
                zadatci = zadatci,
                nazivStatusa = statusi,
                PagingInfo = pagingInfo,
            };
            return View( model);
      
        }

        private async Task PrepareDropDownLists()
        {
            var hr = await ctx.StatusZadatka
                              .Where(d => d.IdStatus == 1)
                              .Select(d => new { d.NazivStatus, d.IdStatus })
                              .FirstOrDefaultAsync();
            var zadatci = await ctx.StatusZadatka
                                  .Where(d => d.IdStatus != 1)
                                  .OrderBy(d => d.NazivStatus)
                                  .Select(d => new { d.NazivStatus, d.IdStatus })
                                  .ToListAsync();
            if (hr != null)
            {
                zadatci.Insert(0, hr);
            }
            ViewBag.ZadatciStatusi = new SelectList(zadatci, nameof(hr.IdStatus), nameof(hr.NazivStatus));

            var hrv = await ctx.Zahtjev
                                  .Where(d => d.IdZahtjev == 1)
                                  .Select(d => new { d.Opis, d.IdZahtjev })
                                  .FirstOrDefaultAsync();
            var projekti = await ctx.Zahtjev
                                  .Where(d => d.IdZahtjev != 1)
                                  .OrderBy(d => d.Opis)
                                  .Select(d => new { d.Opis, d.IdZahtjev })
                                  .ToListAsync();
            if (hrv != null)
            {
                projekti.Insert(0, hrv);
            }
            ViewBag.ZahtjeviPopis = new SelectList(projekti, nameof(hrv.IdZahtjev), nameof(hrv.Opis));

        }

        [HttpGet]
        public async Task<IActionResult> CreateAsync()
        {
            await PrepareDropDownLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Zadatak zadatak)
        {
            logger.LogTrace(JsonSerializer.Serialize(zadatak));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(zadatak);
                    ctx.SaveChanges();
                    logger.LogInformation(new EventId(1000), $"Zadatak {zadatak.IdZadatak} dodan.");

                    TempData[Constants.Message] = $"Zadatak {zadatak.IdZadatak} dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanje novog zadatka: {0}", exc.Message);
                    ModelState.AddModelError(string.Empty, exc.Message);
                    await PrepareDropDownLists();

                    return View(zadatak);
                }
            }
            else
            {
                await PrepareDropDownLists();

                return View(zadatak);
            }
        }


        [HttpGet]
        public async Task <IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var zadatak = ctx.Zadatak.AsNoTracking().Where(d => d.IdZadatak==id).SingleOrDefault();
            if (zadatak == null)
            {
                logger.LogWarning("Ne postoji zadatak s oznakom: {0} ", id);
                return NotFound("Ne postoji zadatak s oznakom: " + id);
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                await PrepareDropDownLists();
                return View(zadatak);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true, string opis = "opis")
        {
            //za različite mogućnosti ažuriranja pogledati
            //attach, update, samo id, ...
            //https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud#update-the-edit-page

            try
            {
                Zadatak zadatak = await ctx.Zadatak
                                  .Where(d => d.IdZadatak==id)
                                  .FirstOrDefaultAsync();
                if (zadatak == null)
                {
                    return NotFound("Neispravan id zadatka: " + id);
                }

                if (await TryUpdateModelAsync<Zadatak>(zadatak, "",
                    d => d.Oibnositelj, d => d.IdStatus, d => d.VrPoc, d => d.VrKraj, d => d.VrKrajOcekivano, d=> d.Vrsta
                ))
                {
                    ViewBag.Page = page;
                    ViewBag.Sort = sort;
                    ViewBag.Ascending = ascending;
                    try
                    {
                        await ctx.SaveChangesAsync();
                        TempData[Constants.Message] = "Zadatak " + id + " ažuriran.";
                        TempData[Constants.ErrorOccurred] = false;
                        return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError(string.Empty, exc.Message);
                        return View(zadatak);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o zadatku nije moguće povezati s forme");
                    return View(zadatak);
                }
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = exc.Message;
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), id);
            }
        }
    }


}
