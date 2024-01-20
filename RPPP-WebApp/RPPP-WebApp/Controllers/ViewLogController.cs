using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Signers;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;

namespace RPPP_WebApp.Controllers;

public class ViewLogController : Controller
{
    private RPPP05Context ctx;

    public ViewLogController(RPPP05Context ctx)
    {
        this.ctx = ctx;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Show(DateTime day)
    {
        List<LogEntry> list = new List<LogEntry>();
        ViewBag.Day = day;
        string format = day.ToString("yyyy-MM-dd");
        DateTime tommorow = day.AddDays(1);

        list = await ctx.Log
            .Where(c => c.Logged >= day && c.Logged < tommorow)
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

