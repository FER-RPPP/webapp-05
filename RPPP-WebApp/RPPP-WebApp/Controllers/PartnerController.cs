using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Drawing.Printing;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.ViewModels;
using RPPP_WebApp.Extensions;


namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// Kontroler za partnere
    /// </summary>
    public class PartnerController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<PartnerController> logger;
        private readonly AppSettings appSettings;

        /// <summary>
        /// Nova instanca klase PartnerController
        /// </summary>
        /// <param name="ctx">Kontekst baze podataka</param>
        /// <param name="options">Postavke aplikacije</param>
        /// <param name="logger">Logger za biljezenje dogadaja</param>
        public PartnerController(RPPP05Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<PartnerController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;
        }

        /// <summary>
        /// Prikazuje popis partnera i njihovih atributa
        /// </summary>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sortiranja</param>
        /// <param name="ascending">Smjer sorta</param>
        /// <returns>View za popis partnera</returns>
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;
            var query = ctx.Partner
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

            var partneri = query
                    .Skip((page - 1) * pagesize)
                    .Take(pagesize)
                    .ToList();

            var tipovi = query
                         .Skip((page - 1) * pagesize)
                         .Take(pagesize)
                         .Select(m => m.IdTipPartneraNavigation.TipPartnera1)
                         .ToList();

            var listasuradnika = partneri
                        .Select(zahtjev => string.Join(",", ctx.Suradnik
                        .Where(z => z.IdPartner == zahtjev.IdPartner)
                        .Select(z => z.IdSuradnik)))
                        .ToList();

            var model = new PartnerViewModel
            {
                partneri = partneri,
                nazivVrste = tipovi,
                PagingInfo = pagingInfo,
                popisSuradnika = listasuradnika
            };
            return View(model);
        }

        /// <summary>
        /// Padajuce liste potrebne za partnera
        /// </summary>
        /// <returns>Task(asinkrono izvodenje)</returns>
        private async Task PrepareDropDownLists()
        {
            var hr = await ctx.TipPartnera
                              .Where(d => d.IdTipPartnera == 1)
                              .Select(d => new { d.TipPartnera1, d.IdTipPartnera })
                              .FirstOrDefaultAsync();
            var partneri = await ctx.TipPartnera
                                  .Where(d => d.IdTipPartnera != 1)
                                  .OrderBy(d => d.TipPartnera1)
                                  .Select(d => new { d.TipPartnera1, d.IdTipPartnera })
                                  .ToListAsync();
            if (hr != null)
            {
                partneri.Insert(0, hr);
            }
            ViewBag.PartneriVrste = new SelectList(partneri, nameof(hr.IdTipPartnera), nameof(hr.TipPartnera1));

            var hrv = await ctx.Projekt
                                  .Where(d => d.IdProjekt == 1)
                                  .Select(d => new { d.Naziv, d.IdProjekt })
                                  .FirstOrDefaultAsync();
            var projekti = await ctx.Projekt
                                  .Where(d => d.IdProjekt != 1)
                                  .OrderBy(d => d.Naziv)
                                  .Select(d => new { d.Naziv, d.IdProjekt })
                                  .ToListAsync();
            if (hrv != null)
            {
                projekti.Insert(0, hrv);
            }
            ViewBag.ProjektiPopis = new SelectList(projekti, nameof(hrv.IdProjekt), nameof(hrv.Naziv));

        }

        /// <summary>
        /// Forma za novog partnera
        /// </summary>
        /// <returns>View za stvaranje novog partnera</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropDownLists();
            return View();
        }

        /// <summary>
        /// Dodaje novog partnera u bazu podataka
        /// </summary>
        /// <param name="partner">Novi partner</param>
        /// <returns>View za popis partnera s dodanim partnerom</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Partner partner)
        {
            logger.LogTrace(JsonSerializer.Serialize(partner));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(partner);
                    ctx.SaveChanges();
                    logger.LogInformation(new EventId(1000), $"Partner {partner.IdPartner} dodan.");

                    TempData[Constants.Message] = $"Partner {partner.IdPartner} dodan.";
                    TempData[Constants.ErrorOccurred] = false;

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja novog partnera: {0}", exc.Message);
                    ModelState.AddModelError(string.Empty, exc.Message);
                    await PrepareDropDownLists();
                    return View(partner);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(partner);
            }
        }

        /// <summary>
        /// Forma za uredivanje partnera
        /// </summary>
        /// <param name="id">Id partnera</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sorta</param>
        /// <param name="ascending">Smjer sorta</param>
        /// <returns>View za uredivanje partnera</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var partner = ctx.Partner.AsNoTracking().Where(d => d.IdPartner == id).SingleOrDefault();
            if (partner == null)
            {
                logger.LogWarning("Ne postoji partner s oznakom: {0} ", id);
                return NotFound("Ne postoji partner s oznakom: " + id);
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                await PrepareDropDownLists();

                return View(partner);
            }
        }

        /// <summary>
        /// Spremanje uredenog partnera u bazu podataka
        /// </summary>
        /// <param name="id">Id partnera</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sorta</param>
        /// <param name="ascending">Smjer sorta</param>
        /// <param name="opis">Opis</param>
        /// <returns>View s popisom partnera</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true, string opis = "opis")
        {
            //za različite mogućnosti ažuriranja pogledati
            //attach, update, samo id, ...
            //https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud#update-the-edit-page

            try
            {
                Partner partner = await ctx.Partner
                                  .Where(d => d.IdPartner == id)
                                  .FirstOrDefaultAsync();
                if (partner == null)
                {
                    return NotFound("Neispravan id partnera: " + id);
                }

                if (await TryUpdateModelAsync<Partner>(partner, "",
                    d => d.Oib, d => d.AdresaPartner, d => d.EmailPartner, d => d.NazivPartner, d => d.Ibanpartner, d => d.IdProjekt, d => d.IdTipPartnera
                ))
                {
                    ViewBag.Page = page;
                    ViewBag.Sort = sort;
                    ViewBag.Ascending = ascending;
                    try
                    {
                        await ctx.SaveChangesAsync();
                        TempData[Constants.Message] = "Partner " + id + " ažuriran.";
                        TempData[Constants.ErrorOccurred] = false;
                        return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError(string.Empty, exc.Message);
                        return View(partner);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o partneru nije moguće povezati s forme");
                    return View(partner);
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
        /// Brise partnera na stranici i u bazi podataka
        /// </summary>
        /// <param name="id">Id partnera</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sorta</param>
        /// <param name="ascending">Smjer sorta</param>
        /// <returns>Vraća na popis partnera</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var partner = ctx.Partner.Find(id);
            var suradnici = ctx.Suradnik.Where(z => z.IdPartner == id).ToList();
            if (partner != null)
            {
                try
                {
                    //foreach (var item in suradnici)
                    //{
                        //ctx.Remove(item);
                    //}
                    int idz = partner.IdPartner;
                    ctx.Remove(partner);
                    ctx.SaveChanges();
                    logger.LogInformation($"Partner {idz} uspješno obrisana");
                    TempData[Constants.Message] = $"partner {idz} uspješno obrisan";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja partnera: " + exc.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja partnera: " + exc.Message);
                }
            }
            else
            {
                logger.LogWarning("Ne postoji zahtjev s oznakom: {0} ", id);
                TempData[Constants.Message] = "Ne postoji zahtjev s oznakom: " + id;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }

        /// <summary>
        /// Prikazuje MD partnera i suradnika(detail)
        /// </summary>
        /// <param name="id">Id partnera</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sorta</param>
        /// <param name="ascending">Smjer sorta</param>
        /// <param name="viewName">Ime View-a</param>
        /// <returns>View za MD formu</returns>
        public async Task<IActionResult> Show(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Show))
        {
            if (id == 0)
            {
                id = (await ctx.Partner.FirstOrDefaultAsync()).IdPartner;

            }

            var partner = await ctx.Partner
                                    .Where(d => d.IdPartner == id)
                                    .Select(d => new Partner
                                    {
                                        IdPartner = d.IdPartner,
                                        Oib = d.Oib,
                                        AdresaPartner = d.AdresaPartner,
                                        Ibanpartner = d.Ibanpartner,
                                        EmailPartner = d.EmailPartner,
                                        NazivPartner = d.NazivPartner,
                                        IdProjekt = d.IdProjekt,
                                        IdTipPartnera = d.IdTipPartnera
                                    })
                                    .FirstOrDefaultAsync();
            int pagesize = 10;
            var query = ctx.Partner
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

            if (partner == null)
            {
                return NotFound($"Partner {id} ne postoji");
            }
            else
            {
                string NazVrste = await ctx.TipPartnera
                                               .Where(p => p.IdTipPartnera == partner.IdTipPartnera)
                                               .Select(p => p.TipPartnera1)
                                               .FirstOrDefaultAsync();

                List<Partner> svipartneri = (await ctx.Partner.ApplySort(sort, ascending).ToListAsync());
                int index = svipartneri.FindIndex(p => p.IdPartner == partner.IdPartner);

                int idprethodnog = -1;
                int idsljedeceg = -1;

                if (index != 0)
                {
                    idprethodnog = svipartneri[index - 1].IdPartner;
                }
                if (index != svipartneri.Count - 1)
                {
                    idsljedeceg = svipartneri[index + 1].IdPartner;
                }


                //učitavanje suradnika
                var suradnici = await ctx.Suradnik
                                      .Where(s => s.IdPartner == partner.IdPartner)
                                      .OrderBy(s => s.IdSuradnik)
                                      .Select(s => new SuradnikDetailPomocniViewModel
                                      {
                                          Oib = s.Oib,
                                          Mobitel = s.Mobitel,
                                          Ime = s.Ime,
                                          Prezime = s.Prezime,
                                          Mail = s.Mail,
                                          Stranka = s.Stranka,
                                          IdPartner = s.IdPartner,
                                          IdSuradnik = s.IdSuradnik,
                                          IdKvalifikacija = s.IdKvalifikacija,
                                          NazivKvalifikacija = s.IdKvalifikacijaNavigation.NazivKvalifikacija
                                      })
                                      .ToListAsync();

               


                var CIJELAPREDAJA = new PartnerSuradnikViewModel
                {
                    partner = partner,
                    TipPartnera = NazVrste,
                    IdPrethPartner = idprethodnog,
                    IdSljedPartner = idsljedeceg,
                    PagingInfo = pagingInfo,
                    Suradnici = suradnici

                };

                //await SetPreviousAndNext(position.Value, filter, sort, ascending);


                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;


                return View(viewName, CIJELAPREDAJA);
            }
        }

        /// <summary>
        /// Forma za uredivanje kroz MD formu
        /// </summary>
        /// <param name="id">Id partnera</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sorta</param>
        /// <param name="ascending">Smjer sorta</param>
        /// <returns>View za uredivanje kroz MD</returns>
        [HttpGet]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            await PrepareDropDownLists();
            var kvalifikacije = await ctx.Kvalifikacija.Select(d => new { d.IdKvalifikacija, d.NazivKvalifikacija }).ToListAsync();
            ViewBag.kvalifikacije = new SelectList(kvalifikacije, nameof(Kvalifikacija.IdKvalifikacija), nameof(Kvalifikacija.NazivKvalifikacija));
            return await Show(id, page, sort, ascending, nameof(Update));
        }

        /// <summary>
        /// Sprema uredeni MD u bazu podataka
        /// </summary>
        /// <param name="model">ViewModel</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="sort">Vrsta sorta</param>
        /// <param name="ascending">Smjer sorta</param>
        /// <returns>View MD forme</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PartnerSuradnikViewModel model, int page = 1, int sort = 1, bool ascending = true)
        {

            Console.WriteLine("Ovo je model: " + model);

            if (ModelState.IsValid)
            {
                try
                {
                    var partner = await ctx.Partner.FindAsync(model.partner.IdPartner);
                    if (partner == null)
                    {
                        return NotFound($"Ne postoji partner s oznakom {model.partner.IdPartner}");
                    }

                    partner.NazivPartner = model.partner.NazivPartner;
                    partner.Oib = model.partner.Oib;
                    partner.AdresaPartner = model.partner.AdresaPartner;
                    partner.Ibanpartner = model.partner.Ibanpartner;
                    partner.EmailPartner = model.partner.EmailPartner;
                    partner.IdTipPartnera = model.partner.IdTipPartnera;

                    List<int> ids = model.Suradnici
                        .Where(d => d.IdSuradnik > 0)
                        .Select(d => d.IdSuradnik)
                        .ToList();

                    ctx.RemoveRange(ctx.Suradnik.Where(d => !ids.Contains(d.IdSuradnik) && d.IdPartner == model.partner.IdPartner));

                    foreach (var item in model.Suradnici)
                    {
                        if (item.IdSuradnik > 0)
                        {
                            var existing = partner.Suradnik.FirstOrDefault(d => d.IdSuradnik == item.IdSuradnik);
                            if (existing != null)
                            {
                                existing.IdKvalifikacija = item.IdKvalifikacija;
                                existing.Ime = item.Ime;
                                existing.Prezime = item.Prezime;
                                existing.Oib = item.Oib;
                                existing.Mobitel = item.Mobitel;
                                existing.Mail = item.Mail;
                                existing.Stranka = item.Stranka;
                            }
                        }
                        else
                        {
                            partner.Suradnik.Add(new Suradnik
                            {
                                IdKvalifikacija = item.IdKvalifikacija,
                                Ime = item.Ime,
                                Prezime = item.Prezime,
                                Oib = item.Oib,
                                Mobitel = item.Mobitel,
                                Mail = item.Mail,
                                Stranka = item.Stranka
                            });
                        }
                    }

                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Partner {model.partner.IdPartner} uspješno ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Update), new { id = model.partner.IdPartner, page = page, sort = sort, ascending = ascending });
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

