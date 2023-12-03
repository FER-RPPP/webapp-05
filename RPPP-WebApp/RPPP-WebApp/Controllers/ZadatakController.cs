using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using RPPP_WebApp.Models;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace RPPP_WebApp.Controllers
{
    public class ZadatakController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<ZadatakController> logger;

        public ZadatakController(RPPP05Context ctx, ILogger<ZadatakController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
        }
        public IActionResult Index()
        {
            var zadatci = ctx.Zadatak
                      .AsNoTracking()
                      .OrderBy(d => d.IdZadatak)
                      .ToList();

            var model = new ZadatakViewModel
            {
                zadatci = zadatci,
            };
            return View("Index", model);
      
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Zadatak zadatak)
        {
            logger.LogTrace(JsonSerializer.Serialize(zadatak));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(zadatak);
                    ctx.SaveChanges();
                    //logger.LogInformation(new EventId(1000), $"Zadatak {zadatak.IdZadatak} dodana.");

                    //TempData[Constants.Message] = $"Država {drzava.NazDrzave} dodana.";
                    //TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    //logger.LogError("Pogreška prilikom dodavanje nove države: {0}", exc.CompleteExceptionMessage());
                    //ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(zadatak);
                }
            }
            else
            {
                return View(zadatak);
            }
        }


        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var zadatak = ctx.Zadatak.AsNoTracking().Where(d => d.IdZadatak==id).SingleOrDefault();
            if (zadatak == null)
            {
                logger.LogWarning("Ne postoji zadatak s oznakom: {0} ", id);
                return NotFound("Ne postoji zadatak s oznakom: " + id);
            }
            else
            {
                //ViewBag.Page = page;
                //ViewBag.Sort = sort;
                //ViewBag.Ascending = ascending;
                return View(zadatak);
            }
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            //za različite mogućnosti ažuriranja pogledati
            //attach, update, samo id, ...
            //https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud#update-the-edit-page

            try
            {
                Zadatak zadatak = await ctx.Zadatak
                                  .Where(d => d.IdZadatak==id)
                                  .FirstOrDefaultAsync();
                if (zadatak == null)
                {
                    return NotFound("Neispravan id zadatka: " + id);
                }

                if (await TryUpdateModelAsync<Zadatak>(zadatak, "",
                    d => d.Oibnositelj, d => d.Vrsta, d => d.VrPoc, d => d.VrKraj, d => d.VrKrajOcekivano
                ))
                {
                    ViewBag.Page = page;
                    ViewBag.Sort = sort;
                    ViewBag.Ascending = ascending;
                    try
                    {
                        await ctx.SaveChangesAsync();
                        ////TempData[Constants.Message] = "Država ažurirana.";
                        ////TempData[Constants.ErrorOccurred] = false;
                        return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                    }
                    catch (Exception exc)
                    {
                        //ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                        return View(zadatak);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o državi nije moguće povezati s forme");
                    return View(zadatak);
                }
            }
            catch (Exception exc)
            {
                //TempData[Constants.Message] = exc.CompleteExceptionMessage();
                //TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), id);
            }
        }
    }


}
