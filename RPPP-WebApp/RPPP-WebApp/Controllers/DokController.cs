using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPPP_WebApp;
using RPPP_WebApp.Models;
using System.Collections.Generic;
using System.Linq;

public class DokController : Controller
{
    private readonly RPPP05Context ctx;
    private readonly ILogger<DokController> logger;
    private readonly AppSettings appSettings;

    public DokController(RPPP05Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<DokController> logger)
    {
        this.ctx = ctx;
        this.logger = logger;
        appSettings = options.Value;
    }

    // API Endpoints

    [HttpGet("/api/Dok")]
    public IEnumerable<Dokument> Get()
    {
        return ctx.Dokument.ToList();
    }

    [HttpGet("/api/Dok/{id}")]
    public ActionResult<Dokument> Get(int id)
    {
        var document = ctx.Dokument.AsNoTracking().FirstOrDefault(d => d.IdDokument == id);

        if (document == null)
        {
            return NotFound();
        }

        return document;
    }

    [HttpPost("/api/Dok")]
    public ActionResult<Dokument> Post([FromBody] Dokument value)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        ctx.Dokument.Add(value);
        ctx.SaveChanges();
        return NoContent();
        
    }

    [HttpPut("/api/Dok/{id}")]
    public ActionResult<Dokument> Put(int id, [FromBody] Dokument value)
    {
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ctx.Dokument.Update(value);
        ctx.SaveChanges();
        return NoContent();
    }

    [HttpDelete("/api/Dok/{id}")]
    public ActionResult<Dokument> Delete(int id)
    {
        
        var document = ctx.Dokument.FirstOrDefault(d => d.IdDokument == id);

        if (document == null)
        {
            return NotFound();
        }

        ctx.Dokument.Remove(document);
        ctx.SaveChanges();
        return NoContent();
    }

    // Views

    public IActionResult Index()
    {
        return View();
    }
}
