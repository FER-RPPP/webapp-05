using Microsoft.AspNetCore.Mvc;
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
    /// kontroller za rad s dokumentima
    /// </summary>
    public class DokumentController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<DokumentController> logger;
        private readonly AppSettings appSettings;

        /// <summary>
        /// instanca kontrollera
        /// </summary>
        /// <param name="ctx">kontekst za bazu</param>
        /// <param name="options">opcije aplikacija</param>
        /// <param name="logger">loger dogadaja</param>
        public DokumentController(RPPP05Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<DokumentController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;
        }

        /// <summary>
        /// Svi dokumenti
        /// </summary>
        /// <param name="page">stranca dokumenata</param>
        /// <param name="sort">vrsta sortiranja</param>
        /// <param name="ascending">smjer sortiranja</param>
        /// <returns>stranicu s svim dokumentima</returns>
        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;

            var query = ctx.Dokument.AsNoTracking();

            int count = query.Count();
            if (count == 0)
            {
                logger.LogInformation("Ne postoji nijedan dokument");
                TempData[Constants.Message] = "Ne postoji nijedan dokument.";
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



            var dokumenti = query.Skip((page - 1) * pagesize).Take(pagesize).ToList();
            var vrstaDokumenta = query.Skip((page - 1) * pagesize).Take(pagesize)
                .Select(p => p.IdVrstaDokNavigation.NazivVrstaDok).ToList();

            List<Projekt> projekts = new List<Projekt>();

            foreach(var doc in dokumenti)
            {
                var p = ctx.Projekt.Find(doc.IdProjekt);
                projekts.Add(p);
            }
            


            var model = new DokumentiViewModel
            {
                Dokumenti = dokumenti,
                PagingInfo = pagingInfo,
                VrstaDokumenta = vrstaDokumenta,
                povezaniProjekti = projekts
            };

            return View(model);
        }

        /// <summary>
        /// funkcija za dohvat podataka za dropdown liste, vrste dokumenata i popis projekata
        /// </summary>
        /// <returns>postavlja podatke u viewbag</returns>
        private async Task PrepareDropdownListDokument()
        {
            var vrste = await ctx.VrstaDokumenta.Select(d => new { d.IdVrstaDok, d.NazivVrstaDok }).ToListAsync();
            ViewBag.vrste = new SelectList(vrste, nameof(VrstaDokumenta.IdVrstaDok), nameof(VrstaDokumenta.NazivVrstaDok));


            var projekti = await ctx.Projekt.Select(d => new { d.IdProjekt, d.Naziv }).ToListAsync();
            ViewBag.projekti = new SelectList(projekti, nameof(Projekt.IdProjekt), nameof(Projekt.Naziv));
        }

        /// <summary>
        /// funkcija za vracanje stranice za dodavanje dokumenta
        /// </summary>
        /// <returns>stranica za dodavanje dokumenata</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropdownListDokument();
            return View();
        }

        /// <summary>
        /// funkcija za dodavanje dokumenta
        /// </summary>
        /// <param name="dokument">novi dokument</param>
        /// <returns>vraca na stranicu ovisno o upjehu akcije</returns>
        [HttpPost]
        public async Task<IActionResult> Create(Dokument dokument)
        {
            logger.LogTrace(JsonSerializer.Serialize(dokument));

            if (ModelState.IsValid)
            {

                //potrebna dodatna validacija
                try
                {
                    ctx.Add(dokument);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Dokument {dokument.NazivDatoteka} dodan.");

                    TempData[Constants.Message] = $"Dokument {dokument.NazivDatoteka} dodan.";
                    TempData[Constants.ErrorOccurred] = false;

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    logger.LogError("Pogreška prilikom dodavanje novog projekta: {0}", ex);
                    ModelState.AddModelError(string.Empty, ex.CompleteExceptionMessage());
                    await PrepareDropdownListDokument();
                    return View(dokument);
                }
            }
            else
            {
                await PrepareDropdownListDokument();
                return View(dokument);
            }
        }

        /// <summary>
        /// funkcija za View stranice za izmjenu dokumenata
        /// </summary>
        /// <param name="id">ID dokumenta</param>
        /// <param name="page">stranica dokumenta</param>
        /// <param name="sort">vrsta sorta</param>
        /// <param name="ascending">smjer sortiranja</param>
        /// <returns>stranicu za izmjenu dokumenata</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var doc = ctx.Dokument.AsNoTracking().Where(d => d.IdDokument == id).SingleOrDefault();
            if (doc == null)
            {
                logger.LogWarning("Ne postoji document s oznakom: {0} ", id);
                return NotFound("Ne postoji dokument s oznakom: " + id);
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                await PrepareDropdownListDokument();
                return View(doc);
            }

        }

        /// <summary>
        /// funkcija za azuriranje dokumenta
        /// </summary>
        /// <param name="id">ID dokumenta</param>
        /// <param name="page">stranica dokumenta</param>
        /// <param name="sort">vrsta sorta</param>
        /// <param name="ascending">smjer sorta</param>
        /// <returns>preusmjerava s obzirom na uspjeh akcije</returns>
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                Dokument dokument = await ctx.Dokument
                                  .Where(p => p.IdDokument == id)
                                  .FirstOrDefaultAsync();
                if (dokument == null)
                {
                    return NotFound("Neispravni id dokumenta: " + id);
                }

                if (await TryUpdateModelAsync<Dokument>(dokument, "",
                    p => p.NazivDatoteka, p => p.VelicinaDokument, p => p.TipDokument, p => p.IdVrstaDok

                ))
                {
                    ViewBag.Page = page;
                    ViewBag.Sort = sort;
                    ViewBag.Ascending = ascending;
                    try
                    {
                        await ctx.SaveChangesAsync();
                        TempData[Constants.Message] = "dokument ažuriran.";
                        TempData[Constants.ErrorOccurred] = false;
                        return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                        return View(dokument);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Nije moguće uzeti podatke s forme.");
                    return View(dokument);
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
        /// funkcij za brisanje dokumenta
        /// </summary>
        /// <param name="id">ID dokumenta za brisanje</param>
        /// <param name="page">stranica</param>
        /// <param name="sort">vrsta sorta</param>
        /// <param name="ascending">smjer sortiranja</param>
        /// <returns>poruku o uspjehu i rediret na stranicu</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var doc = ctx.Dokument.Find(id);

            if (doc != null)
            {
                try
                {
                    int idD = doc.IdDokument;
                    ctx.Remove(doc);
                    ctx.SaveChanges();
                    logger.LogInformation($"Dokument {idD} uspješno obrisan");
                    TempData[Constants.Message] = $"dokument {idD} uspješno obrisan";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja dokumenta: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja dokumenta: " + exc.CompleteExceptionMessage());
                }
            }
            else
            {
                logger.LogWarning("Ne postoji dokument s oznakom: {0} ", id);
                TempData[Constants.Message] = "Ne postoji dokument s oznakom: " + id;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }



    }



}
