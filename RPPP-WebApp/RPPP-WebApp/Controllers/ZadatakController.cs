using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;
using RPPP_WebApp.Models;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.ViewModels;
using Microsoft.Extensions.Options;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// Kontroler za Zadatke
    /// </summary>
    public class ZadatakController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<ZadatakController> logger;
        private readonly AppSettings appSettings;

        /// <summary>
        /// Inicijalizira novu instancu klase ZadatakController/>.
        /// </summary>
        /// <param name="ctx">Kontekst baze podataka</param>
        /// <param name="options">Postavke aplikacije</param>
        /// <param name="logger">Logger za biljezenje dogadaja</param>
        public ZadatakController(RPPP05Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<ZadatakController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;

        }
        
        /// <summary>
        /// Prikazuje popis zadataka s mogucnoscu stranicenja i sortiranja
        /// </summary>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <returns>View s popisom zadataka</returns>
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {

            int pagesize = appSettings.PageSize;
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
                return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
                //return RedirectToAction(nameof(Index),new { page = pagingInfo.TotalPages, sort, ascending });
            }

            query = query.ApplySort(sort, ascending);


            var zadatci = query
                      .Skip((page - 1) * pagesize)
                      .Take(pagesize)
                      .ToList();
            var statusi = query
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

        /// <summary>
        /// Priprema padajućih lista za Create i Edit metode.
        /// </summary>
        /// <returns>Task</returns>
        private async Task PrepareDropDownLists()
        {
            var hrc = await ctx.Suradnik
                              .Where(d => d.Oib.Equals(1))
                              .Select(d => new { S = d.Ime + " "+ d.Prezime + " (OIB: " + d.Oib + ")", d.Oib })
                              .FirstOrDefaultAsync();
            var suradnici = await ctx.Suradnik
                                  .Where(d => !d.Oib.Equals(1))
                                  .OrderBy(d => d.Mail)
                                  .Select(d => new { S = d.Ime + " " + d.Prezime + " (OIB: " + d.Oib + ")", d.Oib })
                                  .ToListAsync();
            if (hrc != null)
            {
                suradnici.Insert(0, hrc);
            }
            ViewBag.ZadatciSuradnici = new SelectList(suradnici, nameof(hrc.Oib), nameof(hrc.S));


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
                                  .Select(d =>  new { v= d.Opis + " (id: " + d.IdZahtjev + ")", d.IdZahtjev } )
                                  .FirstOrDefaultAsync();
            var zahtjevi = await ctx.Zahtjev
                                  .Where(d => d.IdZahtjev != 1)
                                  .OrderBy(d => d.IdZahtjev)
                                  .Select(d =>  new { v= d.Opis + " (id: " + d.IdZahtjev + ")", d.IdZahtjev } )
                                  .ToListAsync();
            if (hrv != null)
            {
                zahtjevi.Insert(0, hrv);
            }
            ViewBag.ZahtjeviPopis = new SelectList(zahtjevi,  nameof(hrv.IdZahtjev),nameof(hrv.v));

        }


        /// <summary>
        /// Prikazuje formu za dodavanje novog zadatka.
        /// </summary>
        /// <returns>View s formom za dodavanje novog zadatka</returns>
        [HttpGet]
        public async Task<IActionResult> CreateAsync()
        {
            await PrepareDropDownLists();
            return View();
        }

        /// <summary>
        /// Dodaje novi zadatak u bazu podataka.
        /// </summary>
        /// <param name="zadatak">Objekt Zadatak</param>
        /// <returns>Redirekcija na Index metodu ili prikazuje grešku za kreiranje novog zadatka</returns>
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
                    Console.WriteLine("Pogreška prilikom dodavanje novog zadatka: {0}" +  exc.InnerException.Message);
                    logger.LogError("Pogreška prilikom dodavanje novog zadatka: {0}" + exc.InnerException.Message);
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

        /// <summary>
        /// Prikazuje formu za uređivanje postojećeg zadatka.
        /// </summary>
        /// <param name="id">ID zadatka</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <returns>View s formom za uređivanje zadatka</returns>
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

        /// <summary>
        /// Ažurira postojeći zadatak u bazi podataka.
        /// </summary>
        /// <param name="id">ID zadatka</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <param name="opis">Opis zadatka</param>
        /// <returns>Redirekcija na Index metodu ili prikazuje grešku za spremanje</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true, string opis = "opis")
        {

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

        /// <summary>
        /// Briše zadatak iz baze podataka.
        /// </summary>
        /// <param name="IdZadatak">ID zadatka</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <returns>Redirekcija na Index metodu</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int IdZadatak, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.OnajKojiBrisem = IdZadatak;
            var zadatak = ctx.Zadatak.Find(IdZadatak);
            if (zadatak != null)
            {
                try
                {
                    int id = zadatak.IdZadatak;
                    ctx.Remove(zadatak);
                    ctx.SaveChanges();
                    logger.LogInformation($"Zadatak {id} uspješno obrisan");
                    TempData[Constants.Message] = $"Zadatak {id} uspješno obrisan";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja zadatka: " + exc.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja zadatka: " + exc.Message);
                }
            }
            else
            {
                logger.LogWarning("Ne postoji zadatak s oznakom: {0} ", IdZadatak);
                TempData[Constants.Message] = "Ne postoji zadatak s oznakom: " + IdZadatak;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }
    }


}
