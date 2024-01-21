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
using System.Collections;
using NLog.Fluent;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PdfRpt.Core.Helper;

namespace RPPP_WebApp.Controllers
{
    public class ZahtjevController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<ZahtjevController> logger;
        private readonly AppSettings appSettings;


        public ZahtjevController(RPPP05Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<ZahtjevController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;

        }
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;
            var query = ctx.Zahtjev
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


            var zahtjevi = query
                      .Skip((page - 1) * pagesize)
                      .Take(pagesize)
                      .ToList();
            var vrste =  query
                         .Skip((page - 1) * pagesize)
                         .Take(pagesize)
                         .Select(m => m.IdVrstaNavigation.NazivVrsta)
                         .ToList();
            //var listazadataka = new List<String>();
            //foreach(var zahtjev in zahtjevi)
            //{
            //    listazadataka.Add(ctx.Zadatak.Where(z => z.IdZahtjev == zahtjev.IdZahtjev)
            //                                .Select(z => z.IdZadatak)
            //                                .ToList().ToString);
            //}
            var listazadataka = zahtjevi
                        .Select(zahtjev => string.Join(",", ctx.Zadatak
                        .Where(z => z.IdZahtjev == zahtjev.IdZahtjev)
                        .Select(z => z.IdZadatak)))
                        .ToList();


            var model = new ZahtjevViewModel
            {
                zadatci = zahtjevi,
                nazivVrste = vrste,
                PagingInfo = pagingInfo,
                popisZadataka = listazadataka
            };
            return View(model);
        }

        private async Task PrepareDropDownLists()
        {

            var hr = await ctx.VrstaZahtjeva
                              .Where(d => d.IdVrsta == 1)
                              .Select(d => new { d.NazivVrsta, d.IdVrsta })
                              .FirstOrDefaultAsync();
            var zahtjevi = await ctx.VrstaZahtjeva
                                  .Where(d => d.IdVrsta != 1)
                                  .OrderBy(d => d.NazivVrsta)
                                  .Select(d => new { d.NazivVrsta, d.IdVrsta })
                                  .ToListAsync();
            if (hr != null)
            {
                zahtjevi.Insert(0, hr);
            }
            ViewBag.ZahtjeviVrste = new SelectList(zahtjevi, nameof(hr.IdVrsta), nameof(hr.NazivVrsta));
            
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

            var hrr = await ctx.StatusZadatka
                              .Where(d => d.IdStatus == 1)
                              .Select(d => new { d.NazivStatus, d.IdStatus })
                              .FirstOrDefaultAsync();
            var zadatci = await ctx.StatusZadatka
                                  .Where(d => d.IdStatus != 1)
                                  .OrderBy(d => d.NazivStatus)
                                  .Select(d => new { d.NazivStatus, d.IdStatus })
                                  .ToListAsync();
            if (hrr != null)
            {
                zadatci.Insert(0, hrr);
            }
            ViewBag.ZadatciStatusi = new SelectList(zadatci, nameof(hrr.IdStatus), nameof(hrr.NazivStatus));

            var hrc = await ctx.Suradnik
                              .Where(d => d.Oib.Equals(1))
                              .Select(d => new { S = d.Ime + " " + d.Prezime + " (OIB: " + d.Oib + ")", d.Oib })
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
        }


        [HttpGet]
        public async Task<IActionResult>  Create()
        {
            await PrepareDropDownLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Zahtjev zahtjev)
        {
            logger.LogTrace(JsonSerializer.Serialize(zahtjev));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(zahtjev);
                    ctx.SaveChanges();
                    logger.LogInformation(new EventId(1000), $"Zahtjev {zahtjev.IdZahtjev} dodan.");

                    TempData[Constants.Message] = $"Zahtjev {zahtjev.IdZahtjev} dodan.";
                    TempData[Constants.ErrorOccurred] = false;

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja novog zahtjeva: {0}", exc.Message);
                    ModelState.AddModelError(string.Empty, exc.Message);
                    await PrepareDropDownLists();
                    return View(zahtjev);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(zahtjev);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var zahtjev = ctx.Zahtjev.AsNoTracking().Where(d => d.IdZahtjev == id).SingleOrDefault();
            if (zahtjev == null)
            {
                logger.LogWarning("Ne postoji zahtjev s oznakom: {0} ", id);
                return NotFound("Ne postoji zahtjev s oznakom: " + id);
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                await PrepareDropDownLists();

                return View(zahtjev);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true, string opis ="opis")
        {
            //za različite mogućnosti ažuriranja pogledati
            //attach, update, samo id, ...
            //https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud#update-the-edit-page

            try
            {
                Zahtjev zahtjev = await ctx.Zahtjev
                                  .Where(d => d.IdZahtjev == id)
                                  .FirstOrDefaultAsync();
                if (zahtjev == null)
                {
                    return NotFound("Neispravan id zahtjeva: " + id);
                }

                if (await TryUpdateModelAsync<Zahtjev>(zahtjev, "",
                    d => d.Opis, d => d.Prioritet, d => d.VrPocetak, d => d.VrKraj, d => d.VrKrajOcekivano, d => d.IdProjekt, d => d.IdVrsta
                ))
                {
                    ViewBag.Page = page;
                    ViewBag.Sort = sort;
                    ViewBag.Ascending = ascending;
                    try
                    {
                        await ctx.SaveChangesAsync();
                        TempData[Constants.Message] = "Zahtjev " + id + " ažuriran.";
                        TempData[Constants.ErrorOccurred] = false;
                        return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError(string.Empty, exc.Message);
                        return View(zahtjev);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o zahtjevu nije moguće povezati s forme");
                    return View(zahtjev);
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
        public IActionResult Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var zahtjev = ctx.Zahtjev.Find(id);
            var zadatci = ctx.Zadatak.Where(z => z.IdZahtjev == id).ToList();
            if (zahtjev != null)
            {
                try
                {
                    foreach (var item in zadatci)
                    {
                        ctx.Remove(item);
                    }
                    int idz = zahtjev.IdZahtjev;
                    ctx.Remove(zahtjev);
                    ctx.SaveChanges();
                    logger.LogInformation($"Zahtjev {idz} uspješno obrisana");
                    TempData[Constants.Message] = $"Zahtjev {idz} uspješno obrisan";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja zahtjeva: " + exc.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja zahtjeva: " + exc.Message);
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

        public async Task<IActionResult> Show(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Show))
        {

            if(id == 0)
            {
                id = (await ctx.Zahtjev.FirstOrDefaultAsync()).IdZahtjev;

            }
            
            var zahtjev = await ctx.Zahtjev
                                    .Where(d => d.IdZahtjev == id)
                                    .Select(d => new Zahtjev
                                    {
                                        IdZahtjev = d.IdZahtjev,
                                        Opis = d.Opis,
                                        Prioritet = d.Prioritet,
                                        VrKraj = d.VrKraj,
                                        VrPocetak = d.VrPocetak,
                                        VrKrajOcekivano = d.VrKrajOcekivano,
                                        IdProjekt = d.IdProjekt,
                                        IdVrsta = d.IdVrsta
                                    })
                                    .FirstOrDefaultAsync();
            ViewBag.Projektic = await ctx.Zahtjev
                                    .Where(d => d.IdProjekt == zahtjev.IdProjekt)
                                    .Select(d =>   d.IdProjektNavigation.Naziv)
                                    .FirstOrDefaultAsync();
                                        
            int pagesize = 10;
            var query = ctx.Zahtjev
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

            if (zahtjev == null)
            {
                return NotFound($"Zahtjev {id} ne postoji");
            }
            else
            {
                 string NazVrste = await ctx.VrstaZahtjeva
                                                .Where(p => p.IdVrsta == zahtjev.IdVrsta)
                                                .Select(p => p.NazivVrsta)
                                                .FirstOrDefaultAsync();

                List<Zahtjev> svizahtjevi = (await ctx.Zahtjev.ApplySort(sort, ascending).ToListAsync());
                int index = svizahtjevi.FindIndex(p => p.IdZahtjev == zahtjev.IdZahtjev);

                int idprethodnog = -1;
                int idsljedeceg = -1;

                if (index != 0) {
                    idprethodnog = svizahtjevi[index - 1].IdZahtjev;
                }
                if(index != svizahtjevi.Count -1) {
                    idsljedeceg = svizahtjevi[index + 1].IdZahtjev;
                 }


                //učitavanje zadataka
                var zadatci = await ctx.Zadatak
                                      .Where(s => s.IdZahtjev == zahtjev.IdZahtjev)
                                      .OrderBy(s => s.IdZadatak)
                                      .Select(s => new ZadatakPomocniViewModel
                                      {
                                          IdZadatak = s.IdZadatak,
                                          VrKraj = s.VrKraj,
                                          VrKrajOcekivano = s.VrKrajOcekivano,
                                          VrPoc = s.VrPoc,
                                          Oibnositelj = s.Oibnositelj,
                                          IdStatus = s.IdStatus,
                                          IdZahtjev = s.IdZahtjev,
                                          Vrsta = s.Vrsta,
                                          NazivStatus = s.IdStatusNavigation.NazivStatus
                                      })
                                      .ToListAsync();

                var zadatcicicici = await ctx.Zadatak
                                     .Where(s => s.IdZahtjev == zahtjev.IdZahtjev)
                                     .OrderBy(s => s.IdZadatak)
                                     .Select(s => new Zadatak
                                     {
                                         IdZadatak = s.IdZadatak,
                                         VrKraj = s.VrKraj,
                                         VrKrajOcekivano = s.VrKrajOcekivano,
                                         VrPoc = s.VrPoc,
                                         Oibnositelj = s.Oibnositelj,
                                         IdStatus = s.IdStatus,
                                         IdZahtjev = s.IdZahtjev,
                                         Vrsta = s.Vrsta
                                     })
                                     .ToListAsync();

                var statusi = ctx.Zadatak.AsNoTracking()
                         .Where(d => d.IdZahtjev == id)
                         .Select(m => m.IdStatusNavigation.NazivStatus)
                         .ToList();

                var model = new ZadatakViewModel
                {
                    zadatci = zadatcicicici,
                    nazivStatusa = statusi,
                    PagingInfo = pagingInfo,
                };


                var CIJELAPREDAJA = new ZahtjevZadatakViewModel
                {
                    zahtjev = zahtjev,
                    NazVrsta = NazVrste,
                    IdPrethZahtjev = idprethodnog,
                    IdSljedZahtjev = idsljedeceg,
                    zadatci = model,
                    Zadatci = zadatci,
                    Statusi = statusi
                
                };



                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                

                return View(viewName, CIJELAPREDAJA);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.ViewName = "Update";
            await PrepareDropDownLists();

            var result = await Show(id, page, sort, ascending, viewName: nameof(Update));

            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ZahtjevZadatakViewModel model, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.ViewName = "Update";
            //if (model.Zadatci == null)
            //{
            //    return NotFound("Nema poslanih podataka");
            //}
            //else
            //{
            //    return NotFound(model.Zadatci);
            //}

            await PrepareDropDownLists();

            ViewBag.Page = page;
            ViewBag.Sort = sort;
            ViewBag.Ascending = ascending;
            if (ModelState.IsValid)
            {
                var zahtjev = await ctx.Zahtjev
                                        .Include(d => d.Zadatak)
                                        .Where(d => d.IdZahtjev == model.zahtjev.IdZahtjev)
                                        .FirstOrDefaultAsync();
                if (zahtjev == null)
                {
                    return NotFound("Ne postoji zahtjev s id-om: " + model.zahtjev.IdZahtjev);
                }

                zahtjev.IdZahtjev = model.zahtjev.IdZahtjev;
                zahtjev.Prioritet = model.zahtjev.Prioritet;
                zahtjev.Opis = model.zahtjev.Opis;
                zahtjev.IdVrsta = model.zahtjev.IdVrsta;
                zahtjev.VrPocetak = model.zahtjev.VrPocetak;
                zahtjev.VrKrajOcekivano = model.zahtjev.VrKrajOcekivano;
                zahtjev.VrKraj = model.zahtjev.VrKraj;

                List<int> idZadataka = model.Zadatci
                                          .Where(s => s.IdZadatak > 0)
                                          .Select(s => s.IdZadatak)
                                          .ToList();

                //if (model.Zadatci == null)
                //    {
                //        return NotFound("Nema poslanih podataka");
                //    }
                //    else
                //    {
                //        return NotFound(model.Zadatci);
                //    }
                    //izbaci sve koje su nisu više u modelu
                ctx.RemoveRange(zahtjev.Zadatak.Where(s => !idZadataka.Contains(s.IdZadatak)));


                foreach (var stavka in model.Zadatci)
                {
                    Zadatak novaStavka; 
                    if (stavka.IdZadatak > 0)
                    {
                        novaStavka = zahtjev.Zadatak.First(s => s.IdZadatak == stavka.IdZadatak);
                        
                    }
                    else
                    {

                        novaStavka = new Zadatak();
                        novaStavka.IdZahtjev = model.zahtjev.IdZahtjev;
                        zahtjev.Zadatak.Add(novaStavka);
                    }

                    var idstatusa = await ctx.StatusZadatka
                                     .Where(s => s.NazivStatus == stavka.NazivStatus)
                                     .Select(s => s.IdStatus)
                                     .FirstOrDefaultAsync();

                    //stavka.IdStatus = idstatusa;


                    int.TryParse(stavka.NazivStatus, out int parsedStatus);

                    //stavka.IdStatus = parsedStatus;



                    novaStavka.Oibnositelj = stavka.Oibnositelj;
                    novaStavka.Vrsta = stavka.Vrsta;
                    novaStavka.IdStatus = (stavka.NazivStatus == null && stavka.IdStatus == novaStavka.IdStatus) ? stavka.IdStatus : parsedStatus;
                    novaStavka.VrPoc = stavka.VrPoc;
                    novaStavka.VrKraj = stavka.VrKraj;
                    novaStavka.VrKrajOcekivano = stavka.VrKrajOcekivano;
                    novaStavka.IdZahtjev = model.zahtjev.IdZahtjev;
                    novaStavka.IdStatusNavigation = stavka.IdStatusNavigation;



                }

                try
                {

                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Zahtjev {zahtjev.IdZahtjev} uspješno ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Show), new
                    {
                        id = zahtjev.IdZahtjev,
                        page,
                        sort,
                        ascending
                    });

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

    }
}
