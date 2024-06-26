﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using System.Text.Json;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.Extensions;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// Projekt controler
    /// </summary>
	public class ProjektController : Controller
	{
		private readonly RPPP05Context ctx;
		private readonly ILogger<ProjektController> logger;
		private readonly AppSettings appSettings;

        /// <summary>
        /// instanca kontrolera
        /// </summary>
        /// <param name="ctx">kontekst za bazu podataka</param>
        /// <param name="options">opcije aplikacije</param>
        /// <param name="logger">loger za događaje</param>
		public ProjektController(RPPP05Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<ProjektController> logger)
		{
			this.ctx = ctx;
			this.logger = logger;
			appSettings = options.Value;
		}


        /// <summary>
        /// funkcija za dohvaćanje svih projekata
        /// </summary>
        /// <param name="page">stranica</param>
        /// <param name="sort">vrsta sorta</param>
        /// <param name="ascending">smjer sorta</param>
        /// <returns>stranica sa svim projektima</returns>
		public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
		{
			int pagesize = appSettings.PageSize;

			var query = ctx.Projekt.AsNoTracking();

			int count = query.Count();
			if (count == 0)
			{
				logger.LogInformation("Ne postoji nijedan projekt");
				TempData[Constants.Message] = "Ne postoji niti jedan projekt.";
				TempData[Constants.ErrorOccurred] = false;
				return RedirectToAction(nameof(Create));
			}


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



			var projekti = query.Skip((page - 1) * pagesize).Take(pagesize).ToList();
			var vrstaProjekta = query.Skip((page - 1) * pagesize).Take(pagesize)
				.Select(p => p.IdTipNavigation.NazivTip).ToList();

			var povezaniDokumenti = projekti
						.Select(projekt => string.Join(",", ctx.Dokument
						.Where(d => d.IdProjekt == projekt.IdProjekt)
						.Select(d => d.IdDokument)))
						.ToList();


			var model = new ProjektiViewModel
			{
				Projekti = projekti,
				PagingInfo = pagingInfo,
				vrstaProjekataList = vrstaProjekta,
				povezaniDokumenti = povezaniDokumenti
			};

			return View(model);
		}

		private async Task PrepareDropdownListProjekt()
		{
			var tipovi = await ctx.TipProjekta.Select(d => new { d.IdTip, d.NazivTip }).ToListAsync();
			ViewBag.Tipovi = new SelectList(tipovi, nameof(TipProjekta.IdTip), nameof(TipProjekta.NazivTip));
		}

        /// <summary>
        /// vraca view za dodavanje novog projekta
        /// </summary>
        /// <returns>vraca view za nove projekte</returns>
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			await PrepareDropdownListProjekt();
			return View();
		}

        /// <summary>
        /// dodaje novi projekt u bazu podataka
        /// </summary>
        /// <param name="projekt">novi projekt</param>
        /// <returns>vraca view sa svim projektima</returns>
		[HttpPost]
		public async Task<IActionResult> Create(Projekt projekt)
		{
			logger.LogTrace(JsonSerializer.Serialize(projekt));
			if (ModelState.IsValid)
			{
                //potrebna dodatna validacija
				try
				{
					ctx.Add(projekt);
					await ctx.SaveChangesAsync();
					logger.LogInformation(new EventId(1000), $"Projekt {projekt.Naziv} dodan.");

					TempData[Constants.Message] = $"Projekt {projekt.Naziv} dodan.";
					TempData[Constants.ErrorOccurred] = false;

					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{

					logger.LogError("Pogreška prilikom dodavanje novog projekta: {0}", ex);
					ModelState.AddModelError(string.Empty, ex.CompleteExceptionMessage());
					await PrepareDropdownListProjekt();
					return View(projekt);
				}
			}
			else
			{
				await PrepareDropdownListProjekt();
				return View(projekt);
			}
		}

        /// <summary>
        /// vraca view za uredivanje projekta
        /// </summary>
        /// <param name="id">oznaka projekta</param>
        /// <param name="page">stranica</param>
        /// <param name="sort">vrsta sorta</param>
        /// <param name="ascending">smjer sorta</param>
        /// <returns>vraca view za uredivanje projekta</returns>
		[HttpGet]
		public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
		{
			var projekt = ctx.Projekt.AsNoTracking().Where(d => d.IdProjekt == id).SingleOrDefault();
			if (projekt == null)
			{
				logger.LogWarning("Ne postoji projekt s oznakom: {0} ", id);
				return NotFound("Ne postoji projekt s oznakom: " + id);
			}
			else
			{
				ViewBag.Page = page;
				ViewBag.Sort = sort;
				ViewBag.Ascending = ascending;
				await PrepareDropdownListProjekt();
				return View(projekt);
			}

		}

        /// <summary>
        /// azurira projekt u bazi podataka
        /// </summary>
        /// <param name="id">oznaka projekta</param>
        /// <param name="page">stranica</param>
        /// <param name="sort">vrsta sorta</param>
        /// <param name="ascending">smjer sorta</param>
        /// <returns>vraca view sa svim projektima i povratne informacije</returns>
		[HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Change(int id, int page = 1, int sort = 1, bool ascending = true)
		{
			try
			{
				Projekt projekt = await ctx.Projekt
								  .Where(p => p.IdProjekt == id)
								  .FirstOrDefaultAsync();
				if (projekt == null)
				{
					return NotFound("Neispravni id projekta: " + id);
				}

				if (await TryUpdateModelAsync<Projekt>(projekt, "",
					p => p.Naziv, p => p.Opis, p => p.VrPocetak, p => p.VrKraj, p => p.IdTip

                ))
				{
					ViewBag.Page = page;
					ViewBag.Sort = sort;
					ViewBag.Ascending = ascending;
					try
					{
						await ctx.SaveChangesAsync();
						TempData[Constants.Message] = "Projekt ažuriran.";
						TempData[Constants.ErrorOccurred] = false;
						return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
					}
					catch (Exception exc)
					{
						ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
						return View(projekt);
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Nije moguće uzeti podatke s forme.");
					return View(projekt);
				}
			}
			catch (Exception exc)
			{
				TempData[Constants.Message] = exc.CompleteExceptionMessage();
				TempData[Constants.ErrorOccurred] = true;
				return RedirectToAction(nameof(Edit), id);
			}
		}

        /// <summary>
        /// brise projekt iz baze podataka
        /// </summary>
        /// <param name="id">projekt</param>
        /// <param name="page">stranica</param>
        /// <param name="sort">vrsta sorta</param>
        /// <param name="ascending">smjer sorta</param>
        /// <returns>vraca povratne informacije i pripadni view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var projekt = ctx.Projekt.Find(id);
            var dokumenti = ctx.Dokument.Where(d => d.IdProjekt == id).ToList();

			var popisZah = ctx.Zahtjev.Where(z => z.IdProjekt == id).ToList();

			foreach(var zah in popisZah)
			{
                var zahtjev = ctx.Zahtjev.Find(zah.IdZahtjev);
                var zadatci = ctx.Zadatak.Where(z => z.IdZahtjev == zah.IdZahtjev).ToList();
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
            }

            if (projekt != null)
            {
                try
                {
                    /*foreach (var doc in dokumenti)
                    {
                        ctx.Remove(doc);
                    }*/
                    int idP = projekt.IdProjekt;
                    ctx.Remove(projekt);
                    ctx.SaveChanges();
                    logger.LogInformation($"Projekt {idP} uspješno obrisan");
                    TempData[Constants.Message] = $"Projekt {idP} uspješno obrisan";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja projekta: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja projekta: " + exc.CompleteExceptionMessage());
                }
            }
            else
            {
                logger.LogWarning("Ne postoji projekt s oznakom: {0} ", id);
                TempData[Constants.Message] = "Ne postoji projekt s oznakom: " + id;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }

        /// <summary>
        /// vraca view za MD prikaz projekta i pripadnih dokumenata
        /// </summary>
        /// <param name="id">ID projekta</param>
        /// <param name="page">stranica</param>
        /// <param name="sort">vrsta sorta</param>
        /// <param name="ascending">smjer sorta</param>
        /// <param name="viewName">koji view vraca</param>
        /// <returns>view za MD prikaz</returns>
        [HttpGet]
        public async Task<IActionResult> MD(int id, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(MD))
        {

            if (id == 0)
            {
                id = (await ctx.Projekt.FirstOrDefaultAsync()).IdProjekt;

            }

            var projekt = await ctx.Projekt
                                    .Where(d => d.IdProjekt == id)
                                    .Select(d => new Projekt
                                    {
                                        IdProjekt = d.IdProjekt,
                                        Opis = d.Opis,
                                        Naziv = d.Naziv,
                                        VrKraj = d.VrKraj,
                                        VrPocetak = d.VrPocetak,
                                        IdTip = d.IdTip,
                                    })
                                    .FirstOrDefaultAsync();
            int pagesize = 10;
            var query = ctx.Projekt
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

            if (projekt == null)
            {
                return NotFound($"Projekt {id} ne postoji");
            }
            else
            {
                string nazivTip = await ctx.TipProjekta
                                               .Where(p => p.IdTip == projekt.IdTip)
                                               .Select(p => p.NazivTip)
                                               .FirstOrDefaultAsync();

                List<Projekt> sviProjekti = (await ctx.Projekt.ApplySort(sort, ascending).ToListAsync());
                int index = sviProjekti.FindIndex(p => p.IdProjekt == projekt.IdProjekt);

                int idprethodnog = -1;
                int idsljedeceg = -1;

                if (index != 0)
                {
                    idprethodnog = sviProjekti[index - 1].IdProjekt;
                }
                if (index != sviProjekti.Count - 1)
                {
                    idsljedeceg = sviProjekti[index + 1].IdProjekt;
                }


                //dokument using DokPomViewModel

                var testDocs = await ctx.Dokument
                                      .Where(s => s.IdProjekt == projekt.IdProjekt)
                                      .OrderBy(s => s.IdDokument)
                                      .Select(s => new DokPomViewModel
                                      {
                                          IdDokument = s.IdDokument,
                                          TipDokument = s.TipDokument,
                                          VelicinaDokument = s.VelicinaDokument,
                                          IdProjekt = s.IdProjekt,
                                          IdVrstaDok = s.IdVrstaDok,
                                          NazivDatoteka = s.NazivDatoteka,
                                          URLdokument = s.URLdokument,
                                          NazivVrstaDok = s.IdVrstaDokNavigation.NazivVrstaDok
                                      })
                                      .ToListAsync();


                var ProjektDokumenti = new MDprojektViewModel
                {
                    projekt = projekt,
                    TipProjekta = nazivTip,
                    IdPrethProjekt = idprethodnog,
                    IdSljedProjekt = idsljedeceg,
                    PagingInfo = pagingInfo,
                    Dokumenti = testDocs
                };


                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;


                return View(viewName, ProjektDokumenti);
            }
        }

        /// <summary>
        /// vraca view za editiranje projekta i pripadnih dokumenata
        /// </summary>
        /// <param name="id">ID projekta</param>
        /// <param name="page">stranica</param>
        /// <param name="sort">vrsta sorta</param>
        /// <param name="ascending">smjer sorta</param>
        /// <returns>Update view</returns>
		[HttpGet]
		public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
		{
            await PrepareDropdownListProjekt();
            var vrste = await ctx.VrstaDokumenta.Select(d => new { d.IdVrstaDok, d.NazivVrstaDok }).ToListAsync();
            ViewBag.vrste = new SelectList(vrste, nameof(VrstaDokumenta.IdVrstaDok), nameof(VrstaDokumenta.NazivVrstaDok));
            return await MD(id, page, sort, ascending, nameof(Update));
		}

        /// <summary>
        /// kontroler za azuriranje projekta i pripadnih dokumenata
        /// </summary>
        /// <param name="model">Viewmodel s projektom i pripadnim dokumentima</param>
        /// <param name="page">stranica</param>
        /// <param name="sort">vrsta sorta</param>
        /// <param name="ascending">smjer sorta</param>
        /// <returns>vraca na isti view s odgovarajucim povratnim informacijama</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(MDprojektViewModel model, int page = 1, int sort = 1, bool ascending = true)
		{

            
			if(ModelState.IsValid)
			{
                try
				{
                    var projekt = await ctx.Projekt.FindAsync(model.projekt.IdProjekt);
                    if(projekt == null)
					{
                        return NotFound($"Ne postoji projekt s oznakom {model.projekt.IdProjekt}");
                    }

                    projekt.Naziv = model.projekt.Naziv;
                    projekt.Opis = model.projekt.Opis;
                    projekt.VrPocetak = model.projekt.VrPocetak;
                    projekt.VrKraj = model.projekt.VrKraj;
                    projekt.IdTip = model.projekt.IdTip;

					List<int> ids = model.Dokumenti
						.Where(d => d.IdDokument > 0)
						.Select(d => d.IdDokument)
						.ToList();

					ctx.RemoveRange(ctx.Dokument.Where(d => !ids.Contains(d.IdDokument) && d.IdProjekt == model.projekt.IdProjekt));

					foreach (var item in model.Dokumenti)
					{
						   if (item.IdDokument > 0)
						    {
                                 var existing = projekt.Dokument.FirstOrDefault(d => d.IdDokument == item.IdDokument);
                                 if (existing != null)
							     {
                                      existing.IdVrstaDok = item.IdVrstaDok;
                                      existing.NazivDatoteka = item.NazivDatoteka;
                                      existing.TipDokument = item.TipDokument;
                                      existing.VelicinaDokument = item.VelicinaDokument;
                                      existing.URLdokument = item.URLdokument;
                                 }
                            }
                            else
						    {
                                 projekt.Dokument.Add(new Dokument
								 {
                                      IdVrstaDok = item.IdVrstaDok,
                                      NazivDatoteka = item.NazivDatoteka,
                                      TipDokument = item.TipDokument,
                                      VelicinaDokument = item.VelicinaDokument,
                                      URLdokument = item.URLdokument
                                 });
                            }
					}

                    await ctx.SaveChangesAsync();
					TempData[Constants.Message] = $"Projekt {model.projekt.Naziv} uspješno ažuriran.";
					TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Update), new {id = model.projekt.IdProjekt, page = page, sort = sort, ascending = ascending });
                }
                catch(Exception exc)
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
