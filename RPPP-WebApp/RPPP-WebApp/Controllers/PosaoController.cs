using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;
using RPPP_WebApp.Models;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Extensions.Selectors;

namespace RPPP_WebApp.Controllers
{
    public class PosaoController : Controller 
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<PosaoController> logger;
        private readonly AppSettings appSettings;
        public PosaoController(RPPP05Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<PosaoController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;
        }

        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {

            int pagesize = appSettings.PageSize;
            var query = ctx.Posao
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

            var poslovi = query
                      .Skip((page - 1) * pagesize)
                      .Take(pagesize)
                      .ToList();

            var nazivi = query
                         .Skip((page - 1) * pagesize)
                         .Take(pagesize)
                         .Select(m => m.IdVrstaPosaoNavigation.NazivPosao)
                         .ToList();

            var suradnici = poslovi
                               .Select(posao => string.Join(",", ctx.Radi
                               .Where(z => z.IdPosao == posao.IdPosao)
                               .Select(z => z.IdSuradnik)))
                                .ToList();

            var model = new PosaoViewModel
            {
                poslovi = poslovi,
                vrstaPosla = nazivi,
                PagingInfo = pagingInfo,
                suradnici = suradnici
            };
            return View(model);

        }
        private async Task PrepareDropDownLists()
        {
            var hr = await ctx.VrstaPosla
                              .Where(d => d.IdVrstaPosao == 1)
                              .Select(d => new { d.NazivPosao, d.IdVrstaPosao })
                              .FirstOrDefaultAsync();
            var poslovi = await ctx.VrstaPosla
                                  .Where(d => d.IdVrstaPosao != 1)
                                  .OrderBy(d => d.NazivPosao)
                                  .Select(d => new { d.NazivPosao, d.IdVrstaPosao })
                                  .ToListAsync();
            if (hr != null)
            {
                poslovi.Insert(0, hr);
            }
            ViewBag.PosloviVrste = new SelectList(poslovi, nameof(hr.IdVrstaPosao), nameof(hr.NazivPosao));

            var hrv = await ctx.Radi
                                  .Where(d => d.IdSuradnik == 1)
                                  .Select(d => new {v =  d.Oib, d.IdSuradnik })
                                  .FirstOrDefaultAsync();

            var suradnici = await ctx.Radi
                                  .Where(d => d.IdSuradnik != 1)
                                  .OrderBy(d => d.Oib)
                                  .Select(d => new {v = d.OibNavigation.Ime + " " + d.OibNavigation.Prezime + " (id: " + d.IdSuradnik + ")", d.IdSuradnik })
                                  .ToListAsync();
            if (hrv != null)
            {
                suradnici.Insert(0, hrv);
            }

            ViewBag.SuradniciPopis = new SelectList(suradnici, nameof(hrv.IdSuradnik), nameof(hrv.v));

        }

        [HttpGet]
        public async Task<IActionResult> CreateAsync()
        {
            await PrepareDropDownLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Posao posao, Suradnik[] selectedSuradnici)
        {
            logger.LogTrace(JsonSerializer.Serialize(posao));

            if (ModelState.IsValid)
            {
                using var transaction = ctx.Database.BeginTransaction();

                try
                {
                    ctx.Add(posao);
                    ctx.SaveChanges();

                    foreach (Suradnik suradnik in selectedSuradnici)
                    {
                        Radi radi = new Radi
                        {
                            Oib = suradnik.Oib,
                            IdPosao = posao.IdPosao
                        };

                        ctx.Add(radi);
                    }

                    ctx.SaveChanges();

                    transaction.Commit();

                    logger.LogInformation(new EventId(1000), $"Posao {posao.IdPosao} dodan.");
                    TempData[Constants.Message] = $"Posao {posao.IdPosao} dodan.";
                    TempData[Constants.ErrorOccurred] = false;

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    transaction.Rollback();

                    logger.LogError("Pogreška prilikom dodavanje novog posla: {0}", exc.Message);
                    ModelState.AddModelError(string.Empty, exc.Message);
                    await PrepareDropDownLists();

                    return View(posao);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(posao);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var posao = ctx.Posao.AsNoTracking().Where(d => d.IdPosao == id).SingleOrDefault();
            if (posao == null)
            {
                logger.LogWarning("Ne postoji posao s oznakom: {0} ", id);
                return NotFound("Ne postoji posao s oznakom: " + id);
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                await PrepareDropDownLists();
                return View(posao);
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
                Posao posao = await ctx.Posao
                                  .Where(d => d.IdPosao == id)
                                  .FirstOrDefaultAsync();
                if (posao == null)
                {
                    return NotFound("Neispravan id posla: " + id);
                }

                if (await TryUpdateModelAsync<Posao>(posao, "",
                    d => d.Uloga, d => d.IdVrstaPosao, d=> d.PredVrTrajanjaDani, d => d.Opis
                ))
                {
                    ViewBag.Page = page;
                    ViewBag.Sort = sort;
                    ViewBag.Ascending = ascending;
                    try
                    {
                        await ctx.SaveChangesAsync();
                        TempData[Constants.Message] = "Posao " + id + " ažuriran.";
                        TempData[Constants.ErrorOccurred] = false;
                        return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError(string.Empty, exc.Message);
                        return View(posao);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o poslu nije moguće povezati s forme");
                    return View(posao);
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
            ViewBag.OnajKojiBrisem = id;
            var posao = ctx.Posao.Find(id);
            if (posao != null)
            {
                try
                {
                    int idPosao = posao.IdPosao;
                    ctx.Remove(posao);
                    ctx.SaveChanges();
                    logger.LogInformation($"Posao {idPosao} uspješno obrisan");
                    TempData[Constants.Message] = $"Posao {idPosao} uspješno obrisan";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja posla: " + exc.Message;
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja posla: " + exc.Message);
                }
            }
            else
            {
                logger.LogWarning("Ne postoji zadatak s oznakom: {0} ", id);
                TempData[Constants.Message] = "Ne postoji zadatak s oznakom: " + id;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }
    }



}