using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using RPPP_WebApp.Models;
using System.Configuration;
using Microsoft.AspNetCore.Http.HttpResults;
using RPPP_WebApp.ViewModels;


namespace RPPP_WebApp.Controllers
{
    public class ProjektController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly ILogger<ProjektController> logger;

        public ProjektController(RPPP05Context ctx, ILogger<ProjektController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
        }

        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = 5;

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
            if (page < 1 || page > pagingInfo.TotalPages)
            {
                return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
            }

            

            var projekti = query
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToList();

            var model = new ProjektiViewModel
            {
                Projekti = projekti,
                PagingInfo = pagingInfo
            };

            return View(model);
        }

        private async Task PrepareDropdownListProjekt()
        {
            var tipovi = await ctx.TipProjekta.Select(d => new {d.IdTip, d.NazivTip}).ToListAsync();
            ViewBag.Tipovi = new SelectList(tipovi, nameof(TipProjekta.IdTip), nameof(TipProjekta.NazivTip));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropdownListProjekt();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Projekt projekt)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    ctx.Add(projekt);
                    await ctx.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }catch(Exception ex) {
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

    }



}
