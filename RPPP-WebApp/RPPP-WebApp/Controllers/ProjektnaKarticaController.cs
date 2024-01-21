using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using System.Text;
using RPPP_WebApp.Extensions.Selectors;
using System.Text.Json;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.Exstensions.Selectors;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// Kontroler za Projektne kartice
    /// </summary>
    public class ProjektnaKarticaController : Controller
    {

        private readonly RPPP05Context ctx;
        private readonly ILogger<ProjektnaKarticaController> logger;
        private readonly AppSettings appSettings;

        /// <summary>
        /// Inicijalizira novu instancu klase ProjektnaKarticaController/>.
        /// </summary>
        /// <param name="ctx">Kontekst baze podataka</param>
        /// <param name="options">Postavke aplikacije</param>
        /// <param name="logger">Logger za biljezenje dogadaja</param>
        public ProjektnaKarticaController(RPPP05Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<ProjektnaKarticaController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;
        }

        /// <summary>
        /// Prikazuje popis projektnih kartica s mogucnoscu stranicenja i sortiranja
        /// </summary>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <returns>View s popisom projektnih kartica</returns>
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

            query = query.ApplySort(sort, ascending);

            var projektnaKartica = query
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToList();
            var vrste = query
                         .Skip((page - 1) * pagesize)
                         .Take(pagesize)
                         .Select(m => m.IdProjektNavigation.Naziv)
                         .ToList();

            var listakartica = projektnaKartica
                       .Select(kartica => string.Join(",", ctx.Transakcija
                       .Where(z => z.SubjektIban == kartica.SubjektIban)
                       .Select(z => z.PrimateljIban)))
                       .ToList();

            var model = new ProjektnaKarticaViewModel
            {
                ProjektnaKartica = projektnaKartica,
                PagingInfo = pagingInfo 
            };

            return View(model);
        }

        /// <summary>
        /// Priprema padajuce liste projektna kartice
        /// </summary>
        /// <returns>Task zadatak za asinkrono izvođenje</returns>
        private async Task PrepareDropDownLists()
        {
            //TODO: napisat funkciju
        }

        /// <summary>
        /// Prikazuje formu za stvaranje nove projektna kartice
        /// </summary>
        /// <returns>Task zadatak za asinkrono izvođenje</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropDownLists();
            return View();
        }

        /// <summary>
        /// Sprema novu projektnu karticu u bazu podataka
        /// </summary>
        /// <param name="proj_kartica">Nova projektna kartica</param>
        /// <returns>Rezultat akcije</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjektnaKartica proj_kartica)
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
                    await PrepareDropDownLists();
                    return View(proj_kartica);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(proj_kartica);
            }
        }


        /// <summary>
        /// Prikazuje formu za uredivanje postojece projektna kartice
        /// </summary>
        /// <param name="id">Identifikator projektna kartice</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <returns>Task zadatak za asinkrono izvodenje</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(string id, int page = 1, int sort = 1, bool ascending = true)
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
                await PrepareDropDownLists();

                return View(kartica);
            }
        }

        /// <summary>
        /// Sprema azurirane podatke o projektnoj kartici u bazu podataka
        /// </summary>
        /// <param name="id">Id projektna kartice</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <param name="opis">Opis projektna kartice</param>
        /// <returns>Task zadatak za asinkrono izvodenje</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, int page = 1, int sort = 1, bool ascending = true, string opis = "opis")
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


        /// <summary>
        /// Brise projektnu karticu iz baze podataka
        /// </summary>
        /// <param name="subjektIBAN">IBAN subjekta</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <returns>Rezultat akcije</returns>
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


        //TREBA DOVRSIT
        /// <summary>
        /// Prikazuje M-D formu projektna kartice i povezane transakcije.
        /// </summary>
        /// <param name="id">Identifikator projektna kartice</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <param name="viewName">Ime pogleda</param>
        /// <returns>Task zadatak za asinkrono izvodenje</returns>
        public async Task<IActionResult> Show(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Show))
        {
            /*if (iban == null)
            {
                iban = (await ctx.ProjektnaKartica.FirstOrDefaultAsync()).SubjektIban;

            }

            var numbers = new StringBuilder();

            foreach (char c in iban)
            {
                if (char.IsDigit(c))
                {
                    numbers.Append(c);
                }
            }
            string nums = numbers.ToString();
            int.TryParse(nums, out int id);*/

            if (id == 0)
            {
                id = (await ctx.ProjektnaKartica.FirstOrDefaultAsync()).IdProjekt;

            }

            var kartica = await ctx.ProjektnaKartica
                                    .Where(d => d.IdProjekt == id)
                                    .Select(d => new ProjektnaKartica
                                    {
                                        SubjektIban = d.SubjektIban,
                                        Saldo = d.Saldo,
                                        Valuta = d.Valuta,
                                        VrijemeOtvaranja = d.VrijemeOtvaranja,
                                        IdProjekt = d.IdProjekt

                                    })
                                    .FirstOrDefaultAsync();
            int pagesize = 10;
            var query = ctx.ProjektnaKartica
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

            if (kartica == null)
            {
                return NotFound($"Projektna kartica {id} ne postoji");
            }
            else
            {
                
                string NazProjekt = await ctx.Projekt
                                               .Where(p => p.IdProjekt == kartica.IdProjekt)
                                               .Select(p => p.Naziv)
                                               .FirstOrDefaultAsync();

                List<ProjektnaKartica> svekartice = await ctx.ProjektnaKartica./*ApplySort(sort, ascending).*/ToListAsync();
                int index = svekartice.FindIndex(p => p.SubjektIban == kartica.SubjektIban);

                
                int idprethodnog = -1;
                int idsljedeceg = -1;
                

                if (index != 0)
                {
                    idprethodnog = svekartice[index - 1].IdProjekt;
                }
                if (index != svekartice.Count - 1)
                {
                    idsljedeceg = svekartice[index + 1].IdProjekt;
                }
                

                //učitavanje transakcije
                var transakcije = await ctx.Transakcija
                                      .Where(s => s.SubjektIban == kartica.SubjektIban)
                                      .OrderBy(s => s.PrimateljIban)
                                      .Select(s => new Transakcija
                                      {
                                          PrimateljIban = s.PrimateljIban,
                                          Vrsta = s.Vrsta,
                                          Opis = s.Opis,
                                          IdTransakcije = s.IdTransakcije,
                                          SubjektIban = s.SubjektIban,
                                          Vrijednost = s.Vrijednost,
                                          Valuta = s.Valuta,
                                      })
                                      .ToListAsync();

                var trans = ctx.Transakcija.AsNoTracking()
                         .Where(d => d.SubjektIban == kartica.SubjektIban)
                         .Select(m => m.IdTransakcijeNavigation.NazivTransakcije)
                         .ToList();

                var model = new TransakcijaViewModel
                {
                    Transakcija = transakcije,
                    nazivTransakcija = trans,
                    PagingInfo = pagingInfo,
                };


                var CIJELAPREDAJA = new ProjektnaKarticaTransakcijaViewModel
                {
                    kartica = kartica,
                    //NazVrsta = NazVrste,
                    
                    IdPrethKartica = idprethodnog,
                    IdSljedKartica = idsljedeceg,
                    
                    transakcije = model

                };


                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;


                return View(viewName, CIJELAPREDAJA);
            }
        }

        //TREBA DOVRSIT
        /// <summary>
        /// Prikazuje formu za azuriranje M-D formu projektna kartice
        /// </summary>
        /// <param name="id">Identifikator projektna kartice</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sortiranja</param>
        /// <returns>Task zadatak za asinkrono izvodenje</returns>
        [HttpGet]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.ViewName = "Update";
            //await PrepareDropDownLists();

            var result = await Show(id, page, sort, ascending, viewName: nameof(Update));

            return result;
        }

    }
}
