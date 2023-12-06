﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


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
    }
}
