using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Controllers
{
    public class PosaoController : Controller 
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<PosaoController> logger;
        public PosaoController(RPPP05Context ctx, ILogger<PosaoController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
        }

        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {

            int pagesize = 10;
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


            var poslovi = ctx.Posao
                      .AsNoTracking()
                      .Skip((page - 1) * pagesize)
                      .Take(pagesize)
                      .OrderBy(d => d.IdPosao)
                      .ToList();
            var nazivi = ctx.Posao.AsNoTracking()
                         .Skip((page - 1) * pagesize)
                         .Take(pagesize)
                         .Select(m => m.IdVrstaPosaoNavigation.NazivPosao)
                         .ToList();

            var model = new PosaoViewModel
            {
                poslovi = poslovi,
                vrstaPosla = nazivi,
                PagingInfo = pagingInfo,
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

            var hrv = await ctx.Posao
                                  .Where(d => d.IdSuradnik.Any(s => s.IdSuradnik == 1))
                                  .Select(d => d.IdPosao + " (id: " + d.IdSuradnik + ")")
                                  .FirstOrDefaultAsync();

            var suradnici = await ctx.Suradnik
                                  .Where(d => d.IdSuradnik != 1)
                                  .OrderBy(d => d.Oib)
                                  .Select(d => d.Ime + d.Prezime + " (id: " + d.IdSuradnik + ")")
                                  .ToListAsync();
            if (hrv != null)
            {
                suradnici.Insert(0, hrv);
            }

            ViewBag.SuradniciPopis = new SelectList(suradnici, nameof(hrv));

        }

        [HttpGet]
        public async Task<IActionResult> CreateAsync()
        {
            await PrepareDropDownLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Posao posao)
        {
            logger.LogTrace(JsonSerializer.Serialize(posao));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(posao);
                    ctx.SaveChanges();
                    logger.LogInformation(new EventId(1000), $"Posao {posao.IdPosao} dodan.");

                    TempData[Constants.Message] = $"Posao  {posao.IdPosao} dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
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
        public IActionResult Delete(int IdPosao, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.OnajKojiBrisem = IdPosao;
            var posao = ctx.Posao.Find(IdPosao);
            if (posao != null)
            {
                try
                {
                    int id = posao.IdPosao;
                    ctx.Remove(posao);
                    ctx.SaveChanges();
                    logger.LogInformation($"Posao {id} uspješno obrisan");
                    TempData[Constants.Message] = $"Posao {id} uspješno obrisan";
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
                logger.LogWarning("Ne postoji posao s oznakom: {0} ", IdPosao);
                TempData[Constants.Message] = "Ne postoji posao s oznakom: " + IdPosao;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }
    }



}