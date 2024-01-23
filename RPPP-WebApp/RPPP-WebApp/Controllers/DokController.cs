using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPPP_WebApp;
using RPPP_WebApp.Models;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// pomocni kontroller za rad s dokumentima(CRUD controller, ali ne radi jTable na frontend)
/// </summary>
public class DokController : Controller
{
    private readonly RPPP05Context ctx;
    private readonly ILogger<DokController> logger;
    private readonly AppSettings appSettings;

    /// <summary>
    /// inicijalizacija kontrollera
    /// </summary>
    /// <param name="ctx">contekst, za nadovezivanje na bazu</param>
    /// <param name="options">postavke aplikacije</param>
    /// <param name="logger">logger za biljezenj dogadaja</param>
    public DokController(RPPP05Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<DokController> logger)
    {
        this.ctx = ctx;
        this.logger = logger;
        appSettings = options.Value;
    }

    // API Endpoints

    /// <summary>
    /// CRUD opreacija za dohvacanje svih dokumenata
    /// </summary>
    /// <returns>svi dokumenti</returns>
    [HttpGet("/api/Dok")]
    public IEnumerable<Dokument> Get()
    {
        return ctx.Dokument.ToList();
    }

    /// <summary>
    /// CRUD operacija za dohvacanje dokumenta po id-u
    /// </summary>
    /// <param name="id">id dokumenta</param>
    /// <returns>dokument s odgovarajucim ID</returns>
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

    /// <summary>
    /// CRUD operacija za dodavanje dokumenta
    /// </summary>
    /// <param name="value">dokument</param>
    /// <returns>status dodavanja, OK - ako uspjelo, BadRequest ako nije</returns>
    [HttpPost("/api/Dok")]
    public ActionResult<Dokument> Post([FromBody] Dokument value)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        ctx.Dokument.Add(value);
        ctx.SaveChanges();
        return Ok();
        
    }

    /// <summary>
    /// CRUD operacija za azuriranje dokumenta
    /// </summary>
    /// <param name="id">id dokumenta</param>
    /// <param name="value">dokument s azuriranim podacima</param>
    /// <returns>status azuriranja, OK - ako uspjelo, BadRequest ako nije </returns>

    [HttpPut("/api/Dok/{id}")]
    public ActionResult<Dokument> Put(int id, [FromBody] Dokument value)
    {
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ctx.Dokument.Update(value);
        ctx.SaveChanges();
        return Ok();
    }


    /// <summary>
    /// CRUD operacija za brisanje dokumenta
    /// </summary>
    /// <param name="id">id dokumenta namjenjenog za brisanje</param>
    /// <returns>Status brisanja</returns>
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
        return Ok();
    }

    // Views
    /// <summary>
    /// stranica s JTable-om za prikaz dokumenata
    /// </summary>
    /// <returns>stranicu s JTable-om</returns>
    public IActionResult Index()
    {
        return View();
    }
}
