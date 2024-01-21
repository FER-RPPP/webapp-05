using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Signers;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;

namespace RPPP_WebApp.Controllers;

/// <summary>
/// Kontroler za pregled unosa zapisnika
/// </summary>
public class ViewLogController : Controller
{
    private RPPP05Context ctx;

    /// <summary>
    /// Inicijalizira novu instancu klase <see cref="ViewLogController"/>.
    /// </summary>
    /// <param name="ctx">Kontekst baze podataka</param>
    public ViewLogController(RPPP05Context ctx)
    {
        this.ctx = ctx;
    }

    /// <summary>
    /// Prikazuje zadani prikaz za unose zapisnika
    /// </summary>
    /// <returns>Zadani prikaz</returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Prikazuje unose zapisnika za odredeni dan
    /// </summary>
    /// <param name="day">Datum za koji se prikazuju unosi</param>
    /// <returns>Prikaz unosa zapisnika</returns>
    public async Task<IActionResult> Show(DateTime day)
    {
        List<LogEntry> list = new List<LogEntry>();
        ViewBag.Day = day;
        string format = day.ToString("yyyy-MM-dd");
        DateTime tomorrow = day.AddDays(1);

        list = await ctx.Log
            .Where(c => c.Logged >= day && c.Logged < tomorrow)
            .Select(c => new LogEntry
            {
                Action = c.Action,
                Controller = c.Logger,
                Level = c.Level,
                Message = c.Message,
                Url = c.Url,
                Id = c.Id,
                Time = c.Logged
            }).ToListAsync();

        return View(list);
    }
}

