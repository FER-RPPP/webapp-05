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


namespace RPPP_WebApp.Controllers
{
    public class ZahtjevController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<ZahtjevController> logger;

        public ZahtjevController(RPPP05Context ctx, ILogger<ZahtjevController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
        }
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
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

            if (page < 1 || page > pagingInfo.TotalPages)
            {
                return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
                //return RedirectToAction(nameof(Index),new { page = pagingInfo.TotalPages, sort, ascending });
            }

            //query = query.ApplySort(sort, ascending);


            var zahtjevi = ctx.Zahtjev
                      .AsNoTracking()
                      .Skip((page - 1) * pagesize)
                      .Take(pagesize)
                      .OrderBy(d => d.IdZahtjev)
                      .ToList();
            var vrste =  ctx.Zahtjev.AsNoTracking()
                         .Skip((page - 1) * pagesize)
                         .Take(pagesize)
                         .Select(m => m.IdVrstaNavigation.NazivVrsta)
                         .ToList();

            var model = new ZahtjevViewModel
            {
                zadatci = zahtjevi,
                nazivVrste = vrste,
                PagingInfo = pagingInfo,
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
        public IActionResult Delete(int IdZahtjev, int page = 1, int sort = 1, bool ascending = true)
        {
            var zahtjev = ctx.Zahtjev.Find(IdZahtjev);
            if (zahtjev != null)
            {
                try
                {
                    int id = zahtjev.IdZahtjev;
                    ctx.Remove(zahtjev);
                    ctx.SaveChanges();
                    logger.LogInformation($"Zahtjev {id} uspješno obrisana");
                    TempData[Constants.Message] = $"Zahtjev {id} uspješno obrisana";
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
                logger.LogWarning("Ne postoji zahtjev s oznakom: {0} ", IdZahtjev);
                TempData[Constants.Message] = "Ne postoji zahtjev s oznakom: " + IdZahtjev;
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

                List<Zahtjev> svizahtjevi = (await ctx.Zahtjev.ToListAsync());
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
                    zadatci = zadatci,
                    nazivStatusa = statusi,
                    PagingInfo = pagingInfo,
                };


                var CIJELAPREDAJA = new ZahtjevZadatakViewModel
                {
                    zahtjev = zahtjev,
                    NazVrsta = NazVrste,
                    IdPrethZahtjev = idprethodnog,
                    IdSljedZahtjev = idsljedeceg,
                    zadatci = model
                
                };

                //await SetPreviousAndNext(position.Value, filter, sort, ascending);


                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                

                return View(viewName, CIJELAPREDAJA);
            }
        }
        //private async Task SetPreviousAndNext(int position, string filter, int sort, bool ascending)
        //{
        //    var query = ctx.vw_Dokumenti.AsQueryable();

        //    DokumentFilter df = new DokumentFilter();
        //    if (!string.IsNullOrWhiteSpace(filter))
        //    {
        //        df = DokumentFilter.FromString(filter);
        //        if (!df.IsEmpty())
        //        {
        //            query = df.Apply(query);
        //        }
        //    }

        //    query = query.ApplySort(sort, ascending);
        //    if (position > 0)
        //    {
        //        ViewBag.Previous = await query.Skip(position - 1).Select(d => d.IdDokumenta).FirstAsync();
        //    }
        //    if (position < await query.CountAsync() - 1)
        //    {
        //        ViewBag.Next = await query.Skip(position + 1).Select(d => d.IdDokumenta).FirstAsync();
        //    }
        //}
    }
}
