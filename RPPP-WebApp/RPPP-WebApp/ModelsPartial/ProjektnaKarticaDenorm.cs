using Microsoft.AspNetCore.Mvc;
using System;

namespace RPPP_WebApp.Models;

public class ProjektnaKarticaDenorm
{
    public string subjektIBAN { get; set; }
    public int Saldo { get; set; }
    public string valuta { get; set; }
    public DateTime vrijemeOtvaranja { get; set; }
    public int idProjekt { get; set; }

    public string KarticaUrl { get; set; }
}
