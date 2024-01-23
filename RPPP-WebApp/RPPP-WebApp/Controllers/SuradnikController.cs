using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;
using RPPP_WebApp.Models;
using System.Reflection.Metadata.Ecma335;
using System.Linq;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.ViewModels;
using RPPP_WebApp.Extensions;
using Org.BouncyCastle.Security;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// Kontroler za suradnike
    /// </summary>
    public class SuradnikController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<SuradnikController> logger;
        private readonly AppSettings appSettings;

        /// <summary>
        /// Inicijalizacija nove instance klase SuradnikController
        /// </summary>
        /// <param name="ctx">Kontekst baze podataka</param>
        /// <param name="options">Postavke aplikacije</param>
        /// <param name="logger">Logger za biljezenje dogadaja</param>
        public SuradnikController(RPPP05Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<SuradnikController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;
        }

        /// <summary>
        /// Prikazuje popis suradnika i njihovih atributa
        /// </summary>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <returns>View za popis suradnika</returns>
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
                               .Select(suradnik => string.Join(",", ctx.Radi
                                .Where(z => z.Oib == suradnik.Oib)
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
    /// <summary>
    /// Priprema padajuce liste za suradnika (popis kvalifikacija i partnera)
    /// </summary>
    /// <returns>Padajuce liste spremne za koristenje</returns>
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

        /// <summary>
        /// Dohvat padajucih lista i prikaz sucelja za dodavanje suradnika
        /// </summary>
        /// <returns>View za dodavanje suradnika</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        { 
            await PrepareDropDownLists();
            return View();
        }

        /// <summary>
        /// Spremanje novog suradnika u bazu podataka
        /// </summary>
        /// <param name="suradnik">Suradnik koji se dodaje u bazu</param>
        /// <returns>Povrat na popis svih suradnika, podatak o uspjesnosti dodavanja</returns>
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

        /// <summary>
        ///  Priprema padajucih lista za uredivanje suradnika i prikaz sucelja za uredivanje ukoliko suradnik postoji
        /// </summary>
        /// <param name="id">Id suradnika koji se ureduje</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <returns>Prikaz sucelja za uredivanje</returns>
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

        /// <summary>
        /// Spremanje uredenog suradnika u bazu
        /// </summary>
        /// <param name="id">Id suradnika koji se ureduje</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <param name="opis">Opis</param>
        /// <returns>Prikaz svih suradnika, podatak o uspjesnosti uredivanja</returns>
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
        /// <summary>
        /// Brisanje suradnika iz baze podataka
        /// </summary>
        /// <param name="oib">Oib suradnika koji se brise</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <returns>Prikaz svih suradnika, podatak o uspjesnosti brisanja</returns>
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

        /// <summary>
        /// Prikaz sucelja za master - detail
        /// </summary>
        /// <param name="id">Id suradnika za prikaz</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <param name="viewName">Ime viewa koji se prikazuje</param>
        /// <returns>Master-detail prikaz</returns>
        public async Task<IActionResult> Show(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Show))
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

                var poslovi = await ctx.Radi
                    .Where(radi => radi.Oib == suradnik.Oib)
                    .OrderBy(posao => posao.IdPosao)
                    .Select(s => new PosaoPomocniViewModel
                    {
                        IdPosao = s.IdPosao,
                        IdVrstaPosao = s.IdPosaoNavigation.IdVrstaPosao,
                        Opis = s.IdPosaoNavigation.Opis,
                        PredVrTrajanjaDani = s.IdPosaoNavigation.PredVrTrajanjaDani,
                        Uloga = s.IdPosaoNavigation.Uloga,
                        NazivPosao = s.IdPosaoNavigation.IdVrstaPosaoNavigation.NazivPosao
                    })
                    .ToListAsync();

                var CIJELAPREDAJA = new MDSuradniciViewModel
                {
                    suradnik = suradnik,
                    kvalifikacija = nazivKvalifikacije,
                    Poslovi = poslovi,
                    IdPrethSuradnik = idprethodnog,
                    IdSljedSuradnik = idsljedeceg,
                    PagingInfo = pagingInfo,
                };


                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;


                return View(viewName, CIJELAPREDAJA);

            }
        }
        /// <summary>
        /// Priprema padajucih listi za uredivanje suradnika i njegovih poslova
        /// </summary>
        /// <param name="id">Id suradnika koji se ureduje</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <returns>Prikaz sucelja za uredivanje</returns>
		[HttpGet]
		public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
		{
			await PrepareDropDownLists();
			var vrste = await ctx.VrstaPosla.Select(d => new { d.IdVrstaPosao, d.NazivPosao }).ToListAsync();
            var kvalifikacije = await ctx.Kvalifikacija.Select(d => new { d.IdKvalifikacija, d.NazivKvalifikacija }).ToListAsync();
            ViewBag.kvalifikacije = new SelectList(kvalifikacije, nameof(Kvalifikacija.IdKvalifikacija), nameof(Kvalifikacija.NazivKvalifikacija));
			ViewBag.vrste = new SelectList(vrste, nameof(VrstaPosla.IdVrstaPosao), nameof(VrstaPosla.NazivPosao));
			return await Show(id, page, sort, ascending, nameof(Update));
		}

        /// <summary>
        /// Spremanje uredenog suradnika i njegovih poslova u bazu podataka
        /// </summary>
        /// <param name="model">Model za prikaz</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">SMjer sortiranja</param>
        /// <returns>Master-detail prikaz uredenog suradnika, podatak o uspjesnosti uredivanja</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(MDSuradniciViewModel model, int page = 1, int sort = 1, bool ascending = true)
		{


			if (ModelState.IsValid)
			{
				try
				{
					var suradnik = await ctx.Suradnik.FindAsync(model.suradnik.IdSuradnik);
					if (suradnik == null)
					{
						return NotFound($"Ne postoji suradnik s oznakom {model.suradnik.IdSuradnik}");
					}

                    suradnik.IdSuradnik = model.suradnik.IdSuradnik;
					suradnik.Oib = model.suradnik.Oib;
					suradnik.Ime = model.suradnik.Ime;
					suradnik.Prezime = model.suradnik.Prezime;
					suradnik.Mail = model.suradnik.Mail;
					suradnik.Mobitel = model.suradnik.Mobitel;
					suradnik.Stranka = model.suradnik.Stranka;
                    suradnik.IdKvalifikacija = model.suradnik.IdKvalifikacija;

					List<int> ids = model.Poslovi
						.Where(d => d.IdPosao > 0)
						.Select(d => d.IdPosao)
						.ToList();

					ctx.RemoveRange(ctx.Posao.Where(d => !ids.Contains(d.IdPosao) && d.Suradnik.Contains(model.suradnik)));

					foreach (var item in model.Poslovi)
					{
						if (item.IdPosao > 0)
						{
							var existing = suradnik.IdPosao.FirstOrDefault(d => d.IdPosao == item.IdPosao);
							if (existing != null)
							{
                                existing.IdPosao = item.IdPosao;
								existing.IdVrstaPosao = item.IdVrstaPosao;
								existing.Opis = item.Opis;
								existing.PredVrTrajanjaDani = item.PredVrTrajanjaDani;
								existing.Uloga = item.Uloga;
							}
						}
						else
						{
							suradnik.IdPosao.Add(new Posao
							{
								IdPosao = item.IdPosao,
								IdVrstaPosao = item.IdVrstaPosao,
								Opis = item.Opis,
								PredVrTrajanjaDani = item.PredVrTrajanjaDani,
								Uloga = item.Uloga
							});
						}
					}

					await ctx.SaveChangesAsync();
					TempData[Constants.Message] = $"Suradnik {model.suradnik.Ime} {model.suradnik.Prezime} uspješno ažuriran.";
					TempData[Constants.ErrorOccurred] = false;
					return RedirectToAction(nameof(Update), new { id = model.suradnik.IdSuradnik, page = page, sort = sort, ascending = ascending });
				}
				catch (Exception exc)
				{
					ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
					return View(nameof(Update), model);
				}
			}
			else
			{
				return View(nameof(Update), model);
			}
		}
	}

}
