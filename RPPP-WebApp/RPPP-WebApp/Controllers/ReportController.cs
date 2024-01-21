using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RPPP_WebApp.Models;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.Exstensions;
using RPPP_WebApp.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt.ColumnsItemsTemplates;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using static iTextSharp.text.pdf.AcroFields;
using RPPP_WebApp.ViewModels.PD_viewModels;

//using OfficeOpenXml;

namespace RPPP_WebApp.Controllers
{
    /// <summary>
    /// Kontroler za izvjesca u pdf/excel
    /// </summary>
    public class ReportController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly IWebHostEnvironment environment;
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>
        /// Inicijalizira novu instancu ReportController-a
        /// </summary>
        /// <param name="ctx">Kontekst podataka baze podataka</param>
        /// <param name="environment">Okolina</param>
        public ReportController(RPPP05Context ctx, IWebHostEnvironment environment)
        {
            this.ctx = ctx;
            this.environment = environment;
        }

        /// <summary>
        /// Prikazuje pocetni prikaz izvjestaja
        /// </summary>
        /// <returns>Rezultat akcije za prikaz pocetnog prikaza</returns>
        public IActionResult Index()
        {
            return View();
        }

        #region Export u Excel Projektne kartice
        /// <summary>
        /// Izvozi projektne kartice u Excel formatu
        /// </summary>
        /// <returns>Excel datoteka sa Projektnim karticama</returns>
        public async Task<IActionResult> ProjektneKarticeExcel()
        {
            var kartice = await ctx.ProjektnaKartica
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdProjekt)
                                  .ToListAsync();
            var kartice2 = await ctx.ProjektnaKartica
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdProjekt)
                                  .Select(d => d.IdProjektNavigation.Naziv)
                                  .ToListAsync();

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis projektnih kartice";
                excel.Workbook.Properties.Author = "RPPP05";
                var worksheet = excel.Workbook.Worksheets.Add("Projektne kartice");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Subjekt IBAN";
                worksheet.Cells[1, 2].Value = "Saldo";
                worksheet.Cells[1, 3].Value = "Vrijeme otvaranja racuna";
                worksheet.Cells[1, 4].Value = "ID projekta";
                worksheet.Cells[1, 5].Value = "Valuta";

                for (int i = 0; i < kartice.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = kartice[i].SubjektIban;
                    worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i + 2, 2].Value = kartice[i].Saldo;
                    worksheet.Cells[i + 2, 3].Value = kartice[i].VrijemeOtvaranja.ToString("d");
                    worksheet.Cells[i + 2, 4].Value = kartice2[i];
                    worksheet.Cells[i + 2, 5].Value = kartice[i].Valuta;
                }

                worksheet.Cells[1, 1, kartice.Count + 1, 4].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "projektneKartice.xlsx");
        }

        #endregion

        #region Export u Excel Transakcije
        /// <summary>
        /// Izvozi transakcije u Excel formatu
        /// </summary>
        /// <returns>Excel datoteka sa Transakcijama</returns>
        public async Task<IActionResult> TransakcijeExcel()
        {
            var transakcije = await ctx.Transakcija
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdTransakcije)
                                  .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis transakcija";
                excel.Workbook.Properties.Author = "RPPP05";
                var worksheet = excel.Workbook.Worksheets.Add("Transakcije");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Primatelj IBAN";
                worksheet.Cells[1, 2].Value = "Opis";
                worksheet.Cells[1, 3].Value = "Vrsta transakcije";
                worksheet.Cells[1, 4].Value = "Subjekt IBAN";
                worksheet.Cells[1, 5].Value = "Id transakcije";
                worksheet.Cells[1, 6].Value = "Vrijednost";
                worksheet.Cells[1, 7].Value = "Valuta";

                for (int i = 0; i < transakcije.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = transakcije[i].PrimateljIban;
                    worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i + 2, 2].Value = transakcije[i].Opis;
                    worksheet.Cells[i + 2, 3].Value = transakcije[i].Vrsta;
                    worksheet.Cells[i + 2, 4].Value = transakcije[i].SubjektIban;

                    string[] sifranik = { "Credit card", "Bank transf.", "ATM", "Paypal" };
                    if (transakcije[i].IdTransakcije == 1)
                    {
                        worksheet.Cells[i + 2, 5].Value = sifranik[0];
                    }
                    else if (transakcije[i].IdTransakcije == 2)
                    {
                        worksheet.Cells[i + 2, 5].Value = sifranik[1];
                    }
                    else if (transakcije[i].IdTransakcije == 3)
                    {
                        worksheet.Cells[i + 2, 5].Value = sifranik[2];
                    }
                    else
                    {
                        worksheet.Cells[i + 2, 5].Value = sifranik[3];
                    }

                    worksheet.Cells[i + 2, 6].Value = transakcije[i].Vrijednost;
                    worksheet.Cells[i + 2, 7].Value = transakcije[i].Valuta;
                }

                worksheet.Cells[1, 1, transakcije.Count + 1, 4].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "transakcije.xlsx");
        }

        #endregion

        #region Export u Excel M-D forma
        /// <summary>
        /// Izvozi M-D formu (Master-Detail) u Excelu
        /// </summary>
        /// <returns>Excel datoteka M-D forme</returns>
        public async Task<IActionResult> ProjektnaKarticaTransakcijeExcel()
        {
            var kartice = await ctx.ProjektnaKartica
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdProjekt)
                                  .ToListAsync();
            var kartice2 = await ctx.ProjektnaKartica
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdProjekt)
                                  .Select(d => d.IdProjektNavigation.Naziv)
                                  .ToListAsync();


            var transakcije = await ctx.Transakcija
                                  .AsNoTracking()
                                  .OrderBy(d => d.SubjektIban)
                                  .ToListAsync();
            byte[] content;

            using (ExcelPackage excel = new ExcelPackage())
            {
                for (int karticaIndex = 0; karticaIndex < kartice.Count; karticaIndex++)
                {
                    var kartica = kartice[karticaIndex];

                    // Create a worksheet for each kartica
                    var worksheet = excel.Workbook.Worksheets.Add($"Kartica_{karticaIndex + 1}");

                    worksheet.Cells[1, 1].Value = "Subjekt IBAN";
                    worksheet.Cells[1, 2].Value = "Saldo";
                    worksheet.Cells[1, 3].Value = "Vrijeme otvaranja racuna";
                    worksheet.Cells[1, 4].Value = "ID projekta";
                    worksheet.Cells[1, 5].Value = "Valuta";

                    worksheet.Cells[2, 1].Value = kartice[karticaIndex].SubjektIban;
                    worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[2, 2].Value = kartice[karticaIndex].Saldo;
                    worksheet.Cells[2, 3].Value = kartice[karticaIndex].VrijemeOtvaranja.ToString("d");
                    worksheet.Cells[2, 4].Value = kartice2[karticaIndex];
                    worksheet.Cells[2, 5].Value = kartice[karticaIndex].Valuta;


                    // Add headers to the worksheet
                    worksheet.Cells[4, 1].Value = "Primatelj IBAN";
                    worksheet.Cells[4, 2].Value = "Opis";
                    worksheet.Cells[4, 3].Value = "Vrsta transakcije";
                    worksheet.Cells[4, 4].Value = "Subjekt IBAN";
                    worksheet.Cells[4, 5].Value = "Id transakcije";
                    worksheet.Cells[4, 6].Value = "Vrijednost";
                    worksheet.Cells[4, 7].Value = "Valuta";

                    // Filter transakcije for the current kartica
                    var karticaTransakcije = transakcije.Where(t => t.SubjektIban == kartica.SubjektIban).ToList();

                    for (int i = 0; i < karticaTransakcije.Count; i++)
                    {
                        var transakcija = karticaTransakcije[i];

                        worksheet.Cells[i + 5, 1].Value = transakcija.PrimateljIban;
                        worksheet.Cells[i + 5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[i + 5, 2].Value = transakcija.Opis;
                        worksheet.Cells[i + 5, 3].Value = transakcija.Vrsta;
                        worksheet.Cells[i + 5, 4].Value = transakcija.SubjektIban;
                        string[] sifranik = { "Credit card", "Bank transf.", "ATM", "Paypal" };
                        if (transakcije[i].IdTransakcije == 1)
                        {
                            worksheet.Cells[i + 5, 5].Value = sifranik[0];
                        }
                        else if (transakcije[i].IdTransakcije == 2)
                        {
                            worksheet.Cells[i + 5, 5].Value = sifranik[1];
                        }
                        else if (transakcije[i].IdTransakcije == 3)
                        {
                            worksheet.Cells[i + 5, 5].Value = sifranik[2];
                        }
                        else
                        {
                            worksheet.Cells[i + 5, 5].Value = sifranik[3];
                        }
                        worksheet.Cells[i + 5, 6].Value = transakcija.Vrijednost;
                        worksheet.Cells[i + 5, 7].Value = transakcija.Valuta;
                    }

                    worksheet.Cells[1, 1, karticaTransakcije.Count + 1, 4].AutoFitColumns();

                }
                content = excel.GetAsByteArray();
                return File(content, ExcelContentType, "master(ProjektnaKartica)-detail(Transakcija).xlsx");
            }



        }

        #endregion

        #region Import iz Excel Projektne kartice
        /// <summary>
        /// Uvozi projektne kartice iz Excel datoteke
        /// </summary>
        /// <param name="importFile">Datoteka koja se uvozi</param>
        /// <returns>Excel datoteka s rezultatima uvoza</returns>
        public async Task<IActionResult> ImportProjektneKartice(IFormFile importFile)
        {
            ExcelPackage result = new ExcelPackage();

            await using (var ms = new MemoryStream())
            {
                await importFile.CopyToAsync(ms);
                using (ExcelPackage import = new ExcelPackage(ms))
                {
                    ExcelWorksheet worksheet = import.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    worksheet.Cells[1, 6].Value = "Status";

                    //treba popravit rowCount
                    for (int row = 2; row <= rowCount; row++)
                    {

                        int idprojekt = await ctx.Projekt.AsNoTracking()
                            .Where(s => s.Naziv.Equals(worksheet.Cells[row, 4].Value))
                            .Select(s => s.IdProjekt)
                            .FirstOrDefaultAsync();
                        if (idprojekt < 1) { break; }

                        ProjektnaKartica kartica = new ProjektnaKartica
                        {
                            SubjektIban = worksheet.Cells[row, 1].Value.ToString().Trim(),
                            Saldo = (double)worksheet.Cells[row, 2].Value,
                            VrijemeOtvaranja = DateTime.Parse(worksheet.Cells[row, 3].Value.ToString()),
                            IdProjekt = idprojekt,
                            Valuta = worksheet.Cells[row, 5].Value.ToString().Trim()
                        };

                        try
                        {
                            ctx.Add(kartica);
                            await ctx.SaveChangesAsync();

                            //logger.LogInformation($"Kartica uspješno dodana. IBAN={kartica.SubjektIban}");
                            worksheet.Cells[row, 6].Value = "ADDED";
                        }
                        catch (Exception exc)
                        {
                            worksheet.Cells[row, 6].Value = "ERROR";
                            //logger.LogError("Pogreška prilikom dodavanja nove kartice: {0}", exc.CompleteExceptionMessage());
                            ModelState.AddModelError(string.Empty, exc.Message);
                        }
                    }

                    result.Workbook.Worksheets.Add("StatusiDodavanjaProjektneKartice", worksheet);

                }
            }
            return File(result.GetAsByteArray(), ExcelContentType, "StatusiDodavanja.xlsx");
        }
        #endregion


        #region PDF Projektne kartice
        /// <summary>
        /// Generira PDF izvjestaj s projektnim karticama
        /// </summary>
        /// <returns>PDF izvjestaja s projektima</returns>
        public async Task<IActionResult> ProjektneKarticePDF()
        {
            string naslov = "Popis projektnih kartica";
            var kartice = await ctx.ProjektnaKartica
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdProjekt)
                                  .ToListAsync();
            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion

            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(kartice));

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(ProjektnaKartica.SubjektIban));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(4);
                    column.HeaderCell("Subjekt IBAN");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(ProjektnaKartica.Saldo));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(2);
                    column.HeaderCell("Saldo");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(ProjektnaKartica.Valuta));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(2);
                    column.HeaderCell("Valuta");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(ProjektnaKartica.VrijemeOtvaranja));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(4);
                    column.HeaderCell("Vrijeme otvaranja");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(ProjektnaKartica.IdProjekt));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(2);
                    column.HeaderCell("ID Projekt");
                });
            });

            #endregion

            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=projektne_kartice.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region PDF Transakcije
        /// <summary>
        /// Generira PDF izvjestaj s transakcijama
        /// </summary>
        /// <returns>PDF izvjestaja s transakcijama</returns>
        public async Task<IActionResult> TransakcijePDF()
        {
            string naslov = "Popis transakcija";
            var kartice = await ctx.Transakcija
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdTransakcije)
                                  .ToListAsync();
            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion

            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(kartice));

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Transakcija.PrimateljIban));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(4);
                    column.HeaderCell("Primatelj IBAN");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Transakcija.Opis));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(4);
                    column.HeaderCell("Opis");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Transakcija.Vrsta));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(4);
                    column.HeaderCell("Vrsta transakcije");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Transakcija.SubjektIban));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(4);
                    column.HeaderCell("Subjekt IBAN");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Transakcija.IdTransakcije));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(4);
                    column.HeaderCell("ID transakcije");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Transakcija.Vrijednost));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(4);
                    column.HeaderCell(" Vrijednost");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Transakcija.Valuta));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(7);
                    column.Width(4);
                    column.HeaderCell("Valuta");
                });
            });

            #endregion

            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=transakcije.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region PDF M-D forma
        /// <summary>
        /// Generira PDF izvjsštaj M-D forme (Master-Detail) s projektima i transakcijama
        /// </summary>
        /// <returns>PDF izvjestaja M-D forme</returns>
        public async Task<IActionResult> ProjektnaKarticaTransakcijePDF()
        {
            var transakcije = await ctx.Transakcija
                                  .Select(u => new ProjektnaKarticaDenorm
                                  {
                                      primateljIBAN = u.PrimateljIban,
                                      subjektIBAN = u.SubjektIban,
                                      opis = u.Opis,
                                      valuta = u.Valuta,
                                      vrsta = u.Vrsta,
                                      vrijednost = u.Vrijednost

                                  })
                                  .ToListAsync();

            string title = $"M-D form ProjektneKarticeTransakcije";

            transakcije.ForEach(s => s.KarticaUrl = Url.Action("Edit", "Projektna Kartica", new { iban = s.subjektIBAN }));
            PdfReport report = CreateReport(title);

            #region Header and footer
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.CustomHeader(new MasterDetailsHeaders(title)
                {
                    PdfRptFont = header.PdfFont
                });
            });
            #endregion

            #region Set datasource and define columns

            //report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(kartice));
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(transakcije));

            report.MainTableColumns(columns =>
            {
                #region Stupci po kojima se grupira
                columns.AddColumn(column =>
                {
                    column.PropertyName<ProjektnaKarticaDenorm>(u => u.subjektIBAN);
                    column.Group(
                        (val1, val2) =>
                        {
                            return val1 == val2;
                        });
                });

                #endregion

                columns.AddColumn(column =>
                {
                    column.PropertyName<ProjektnaKarticaDenorm>(x => x.subjektIBAN);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(4);
                    column.HeaderCell("SubjektIBAN", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<ProjektnaKarticaDenorm>(x => x.Saldo);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Saldo", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<ProjektnaKarticaDenorm>(x => x.valuta);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Valuta", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<ProjektnaKarticaDenorm>(x => x.vrijemeOtvaranja);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(4);
                    column.HeaderCell("Vrijeme otvaranja", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<ProjektnaKarticaDenorm>(x => x.idProjekt);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("ID Projekta", horizontalAlignment: HorizontalAlignment.Center);
                });


                columns.AddColumn(column =>
                {
                    column.PropertyName<ProjektnaKarticaDenorm>(x => x.primateljIBAN);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Primatelj IBAN", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<ProjektnaKarticaDenorm>(x => x.opis);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Opis", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<ProjektnaKarticaDenorm>(x => x.vrsta);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Vrsta trans.", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<ProjektnaKarticaDenorm>(x => x.subjektIBAN);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Subjekt IBAN", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<ProjektnaKarticaDenorm>(x => x.idTransakcija);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("ID Transakcije", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<ProjektnaKarticaDenorm>(x => x.vrijednost);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Vrijednost", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<ProjektnaKarticaDenorm>(x => x.valuta);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Valuta", horizontalAlignment: HorizontalAlignment.Center);
                });

            });

            #endregion

            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=documents.pdf");
                return File(pdf, "application/pdf");
            }
            else
                return NotFound();
        }

        #endregion


        #region CreateReport funkcija
        /// <summary>
        /// Stvara novi PDF izvjestaj s postavkama
        /// </summary>
        /// <param name="naslov">Naslov dokumenta.</param>
        private PdfReport CreateReport(string naslov)
        {
            var pdf = new PdfReport();

            pdf.DocumentPreferences(doc =>
            {
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata
                {
                    Author = "RPPP05",
                    Application = "RPPP05 Web-app",
                    Title = naslov
                });
                doc.Compression(new CompressionSettings
                {
                    EnableCompression = true,
                    EnableFullCompression = true
                });
            })
            //fix za linux https://github.com/VahidN/PdfReport.Core/issues/40
            .DefaultFonts(fonts =>
            {
                fonts.Path(Path.Combine(environment.WebRootPath, "fonts", "verdana.ttf"),
                           Path.Combine(environment.WebRootPath, "fonts", "tahoma.ttf"));
                fonts.Size(9);
                fonts.Color(System.Drawing.Color.Black);
            })
            //
            .MainTableTemplate(template =>
            {
                template.BasicTemplate(BasicTemplate.ProfessionalTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
                //table.NumberOfDataRowsPerPage(20);
                table.GroupsPreferences(new GroupsPreferences
                {
                    GroupType = GroupType.HideGroupingColumns,
                    RepeatHeaderRowPerGroup = true,
                    ShowOneGroupPerPage = true,
                    SpacingBeforeAllGroupsSummary = 5f,
                    NewGroupAvailableSpacingThreshold = 150,
                    SpacingAfterAllGroupsSummary = 5f
                });
                table.SpacingAfter(4f);
            });

            return pdf;
        }
        #endregion

        #region Master-detail header
        /// <summary>
        /// Implementacija IPageHeader sucelja za prikaz zaglavlja u M-D (Master-Detail) izvjestajima
        /// </summary>
        public class MasterDetailsHeaders : IPageHeader
        {
            private readonly string title;
            /// <summary>
            /// Inicijalizira novu instancu MasterDetailsHeaders klasea
            /// </summary>
            /// <param name="title">Naslov izvjestaja</param>
            public MasterDetailsHeaders(string title)
            {
                this.title = title;
            }

            /// <summary>
            /// Postavlja font za PDF izvjestaj
            /// </summary>
            public IPdfFont PdfRptFont { set; get; }

            /// <summary>
            /// Renderira grupno zaglavlje stranice
            /// </summary>
            public PdfGrid RenderingGroupHeader(iTextSharp.text.Document pdfDoc, PdfWriter pdfWriter, IList<CellData> newGroupInfo, IList<SummaryCellData> summaryData)
            {
                var subjektIBAN = newGroupInfo.GetSafeStringValueOf(nameof(ProjektnaKarticaDenorm.subjektIBAN));
                var ProjektnaKarticaURL = newGroupInfo.GetValueOf(nameof(ProjektnaKarticaDenorm.KarticaUrl));
                var Saldo = newGroupInfo.GetSafeStringValueOf(nameof(ProjektnaKarticaDenorm.Saldo));
                var Valuta = newGroupInfo.GetSafeStringValueOf(nameof(ProjektnaKarticaDenorm.valuta));
                var VrijemeOtvaranja = (DateTime)newGroupInfo.GetValueOf(nameof(ProjektnaKarticaDenorm.vrijemeOtvaranja));
                var IdProjekt = (int)newGroupInfo.GetValueOf(nameof(ProjektnaKarticaDenorm.idProjekt));


                var table = new PdfGrid(relativeWidths: new[] { 2f, 2f, 2f, 2f, 2f, 2f }) { WidthPercentage = 100 };

                table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Document Id:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.TableRowData = newGroupInfo; //postavi podatke retka za ćeliju
                        var cellTemplate = new HyperlinkField(BaseColor.Black, false)
                        {
                            TextPropertyName = nameof(ProjektnaKarticaDenorm.subjektIBAN),
                            NavigationUrlPropertyName = nameof(ProjektnaKarticaDenorm.KarticaUrl),
                            BasicProperties = new CellBasicProperties
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                PdfFontStyle = DocumentFontStyle.Bold,
                                PdfFont = PdfRptFont
                            }
                        };

                        cellData.CellTemplate = cellTemplate;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Saldo:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = Saldo;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Subjekt IBAN:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = subjektIBAN;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    });

                table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Valuta:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = Valuta;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Vrijeme otvaranja:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = VrijemeOtvaranja;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                        cellProperties.DisplayFormatFormula = obj => ((DateTime)obj).ToString("dd.MM.yyyy");
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "ID Projekt:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = IdProjekt;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    });
                return table.AddBorderToTable(borderColor: BaseColor.LightGray, spacingBefore: 5f);
            }

            /// <summary>
            /// Renderira zaglavlje izvjestaja
            /// </summary>
            public PdfGrid RenderingReportHeader(iTextSharp.text.Document pdfDoc, PdfWriter pdfWriter, IList<SummaryCellData> summaryData)
            {
                var table = new PdfGrid(numColumns: 1) { WidthPercentage = 100 };
                table.AddSimpleRow(
                   (cellData, cellProperties) =>
                   {
                       cellData.Value = title;
                       cellProperties.PdfFont = PdfRptFont;
                       cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                       cellProperties.HorizontalAlignment = HorizontalAlignment.Center;
                   });
                return table.AddBorderToTable();
            }
        }

        #endregion

        public async Task<IActionResult> ZahtjevExcel()
        {
            var zahtjevi = await ctx.Zahtjev
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdZahtjev)
                                  .ToListAsync();

            var vrste = await ctx.Zahtjev.AsNoTracking().OrderBy(d => d.IdZahtjev)
                                  .Select(d => d.IdVrstaNavigation.NazivVrsta)
                                  .ToListAsync();

            var projekti = await ctx.Zahtjev.AsNoTracking().OrderBy(d => d.IdZahtjev)
                                  .Select(d => d.IdProjektNavigation.Naziv)
                                  .ToListAsync();

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis zahtjeva";
                excel.Workbook.Properties.Author = "RPPP05";
                var worksheet = excel.Workbook.Worksheets.Add("Zahtjevi");

                //First add the headers
                worksheet.Cells[1, 1].Value = "ID zahtjeva";
                worksheet.Cells[1, 2].Value = "ID Projekta";
                worksheet.Cells[1, 3].Value = "Naziv Vrste";
                worksheet.Cells[1, 4].Value = "Opis";
                worksheet.Cells[1, 5].Value = "Prioritet";
                worksheet.Cells[1, 6].Value = "Vrijeme početka";
                worksheet.Cells[1, 7].Value = "Vrijeme kraja";
                worksheet.Cells[1, 8].Value = "Očekivano vrijeme kraja";


                for (int i = 0; i < zahtjevi.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = zahtjevi[i].IdZahtjev;
                    worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i + 2, 2].Value = projekti[i];
                    worksheet.Cells[i + 2, 3].Value = vrste[i];
                    worksheet.Cells[i + 2, 4].Value = zahtjevi[i].Opis;
                    worksheet.Cells[i + 2, 5].Value = zahtjevi[i].Prioritet;
                    worksheet.Cells[i + 2, 6].Value = zahtjevi[i].VrPocetak.ToString("g");
                    worksheet.Cells[i + 2, 7].Value = zahtjevi[i].VrKraj.HasValue ? zahtjevi[i].VrKraj.Value.ToString("g") : "";
                    worksheet.Cells[i + 2, 8].Value = zahtjevi[i].VrKrajOcekivano.ToString("g");

                }

                worksheet.Cells[1, 1, zahtjevi.Count + 1, 8].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "Zahtjevi.xlsx");
        }
        public async Task<IActionResult> ZadatakExcel()
        {
            var zadatci = await ctx.Zadatak
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdZadatak)
                                  .ToListAsync();

            var statusi = await ctx.Zadatak.AsNoTracking().OrderBy(d => d.IdZahtjev)
                                  .Select(d => d.IdStatusNavigation.NazivStatus)
                                  .ToListAsync();

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis Zadataka";
                excel.Workbook.Properties.Author = "RPPP05";
                var worksheet = excel.Workbook.Worksheets.Add("Zadatci");

                //First add the headers
                worksheet.Cells[1, 1].Value = "ID zadatka";
                worksheet.Cells[1, 2].Value = "Naziv";
                worksheet.Cells[1, 3].Value = "OIB nositelja";
                worksheet.Cells[1, 4].Value = "Status zadatka";
                worksheet.Cells[1, 5].Value = "Vrijeme početka";
                worksheet.Cells[1, 6].Value = "Vrijeme kraja";
                worksheet.Cells[1, 7].Value = "Vrijeme očekivanog kraja";

                for (int i = 0; i < zadatci.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = zadatci[i].IdZadatak;
                    worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i + 2, 2].Value = zadatci[i].Vrsta;
                    worksheet.Cells[i + 2, 3].Value = zadatci[i].Oibnositelj;
                    worksheet.Cells[i + 2, 4].Value = statusi[i];
                    worksheet.Cells[i + 2, 5].Value = zadatci[i].VrPoc.ToString("g");
                    worksheet.Cells[i + 2, 6].Value = zadatci[i].VrKraj.HasValue ? zadatci[i].VrKraj.Value.ToString("g") : "";
                    worksheet.Cells[i + 2, 7].Value = zadatci[i].VrKrajOcekivano.ToString("g");
                }

                worksheet.Cells[1, 1, zadatci.Count + 1, 7].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "Zadatci.xlsx");
        }
        public async Task<IActionResult> ZahtjeviZadatciExcel()
        {
            var zahtjevi = await ctx.Zahtjev
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdZahtjev)
                                  .ToListAsync();
            var vrste = await ctx.Zahtjev
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdZahtjev)
                                  .Select(d => d.IdVrstaNavigation.NazivVrsta)
                                  .ToListAsync();
            var projekti = await ctx.Zahtjev
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdZahtjev)
                                  .Select(d => d.IdProjektNavigation.Naziv)
                                  .ToListAsync();


            var zadatcisvi = await ctx.Zadatak
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdZadatak)
                                  .ToListAsync();
            byte[] content;

            using (ExcelPackage excel = new ExcelPackage())
            {
                for (int i = 0; i < zahtjevi.Count; i++)
                {
                    var zahtjev = zahtjevi[i];

                    // Create a worksheet for each kartica
                    var worksheet = excel.Workbook.Worksheets.Add($"Zahtjev_{i + 1}");


                    worksheet.Cells[1, 1].Value = "ID zahtjeva";
                    worksheet.Cells[1, 2].Value = "ID Projekta";
                    worksheet.Cells[1, 3].Value = "Naziv Vrste";
                    worksheet.Cells[1, 4].Value = "Opis";
                    worksheet.Cells[1, 5].Value = "Prioritet";
                    worksheet.Cells[1, 6].Value = "Vrijeme početka";
                    worksheet.Cells[1, 7].Value = "Vrijeme kraja";
                    worksheet.Cells[1, 8].Value = "Očekivano vrijeme kraja";


                    worksheet.Cells[2, 1].Value = zahtjevi[i].IdZahtjev;
                    worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[2, 2].Value = projekti[i];
                    worksheet.Cells[2, 3].Value = vrste[i];
                    worksheet.Cells[2, 4].Value = zahtjevi[i].Opis;
                    worksheet.Cells[2, 5].Value = zahtjevi[i].Prioritet;
                    worksheet.Cells[2, 6].Value = zahtjevi[i].VrPocetak.ToString("g");
                    worksheet.Cells[2, 7].Value = zahtjevi[i].VrKraj.HasValue ? zahtjevi[i].VrKraj.Value.ToString("g") : "";
                    worksheet.Cells[2, 8].Value = zahtjevi[i].VrKrajOcekivano.ToString("g");



                    // Add headers to the worksheet
                    worksheet.Cells[4, 1].Value = "ID zadatka";
                    worksheet.Cells[4, 2].Value = "Naziv";
                    worksheet.Cells[4, 3].Value = "OIB nositelja";
                    worksheet.Cells[4, 4].Value = "Status zadatka";
                    worksheet.Cells[4, 5].Value = "Vrijeme početka";
                    worksheet.Cells[4, 6].Value = "Vrijeme kraja";
                    worksheet.Cells[4, 7].Value = "Vrijeme očekivanog kraja";

                    // Filter transakcije for the current kartica
                    var zadatci = zadatcisvi.Where(t => t.IdZahtjev == zahtjev.IdZahtjev).ToList();

                    for (int j = 0; j < zadatci.Count; j++)
                    {

                        var statusi = ctx.Zadatak.AsNoTracking().Where(d => d.IdZadatak == zadatci[j].IdZadatak)
                                                            .Select(d => d.IdStatusNavigation.NazivStatus).FirstOrDefault();


                        worksheet.Cells[j + 5, 1].Value = zadatci[j].IdZadatak;
                        worksheet.Cells[j + 5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[j + 5, 2].Value = zadatci[j].Vrsta;
                        worksheet.Cells[j + 5, 3].Value = zadatci[j].Oibnositelj;
                        worksheet.Cells[j + 5, 4].Value = statusi;
                        worksheet.Cells[j + 5, 5].Value = zadatci[j].VrPoc.ToString("g");
                        worksheet.Cells[j + 5, 6].Value = zadatci[j].VrKraj.HasValue ? zadatci[j].VrKraj.Value.ToString("g") : "";
                        worksheet.Cells[j + 5, 7].Value = zadatci[j].VrKrajOcekivano.ToString("g");
                    }

                    worksheet.Cells[1, 1, zadatci.Count + 1, 7].AutoFitColumns();

                }
                content = excel.GetAsByteArray();
                return File(content, ExcelContentType, "master(Zahtjev)-detail(Zadatak).xlsx");
            }
        }
        public async Task<IActionResult> ZahtjevPDF()
        {
            string naslov = "Popis zahtjeva";

            var zadatci = await ctx.Zahtjev
                                      .AsNoTracking()
                                      .Select(s => new ZahtjevPomocniViewModel
                                      {
                                          IdZahtjev = s.IdZahtjev,
                                          VrKraj = s.VrKraj,
                                          VrKrajOcekivano = s.VrKrajOcekivano,
                                          VrPocetak = s.VrPocetak,
                                          Prioritet = s.Prioritet,
                                          Opis = s.Opis,
                                          IdProjekt = s.IdProjekt,
                                          IdVrsta = s.IdVrsta,
                                          NazivVrsta = s.IdVrstaNavigation.NazivVrsta
                                      })
                                      .ToListAsync();


            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion

            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(zadatci));

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zahtjev.IdZahtjev));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("ID Zahtjeva");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zahtjev.IdProjekt));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(false);
                    column.Order(2);
                    column.Width(1);
                    column.HeaderCell("ID Projekta");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zahtjev.IdVrstaNavigation.NazivVrsta));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(3);
                    column.HeaderCell("Vrsta");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zahtjev.Opis));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(3);
                    column.HeaderCell("Opis");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zahtjev.Prioritet));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(2);
                    column.HeaderCell("Prioritet");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zahtjev.VrPocetak));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(2);
                    column.HeaderCell("Vrijeme početka");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zahtjev.VrKraj));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(7);
                    column.Width(2);
                    column.HeaderCell("Vrijeme kraja");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zahtjev.VrKrajOcekivano));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(8);
                    column.Width(2);
                    column.HeaderCell("Očekivano vrijeme kraja");
                });
            });

            #endregion

            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=zahtjevi.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }
        public async Task<IActionResult> ZadatakPDF()
        {
            string naslov = "Popis zadataka";

            var zadatci = await ctx.Zadatak
                      .OrderBy(s => s.IdZadatak)
                      .Select(s => new ZadatakPomocniViewModel
                      {
                          IdZadatak = s.IdZadatak,
                          VrKraj = s.VrKraj,
                          VrKrajOcekivano = s.VrKrajOcekivano,
                          VrPoc = s.VrPoc,
                          Oibnositelj = s.Oibnositelj,
                          IdStatus = s.IdStatus,
                          IdZahtjev = s.IdZahtjev,
                          Vrsta = s.Vrsta,
                          NazivStatus = s.IdStatusNavigation.NazivStatus
                      })
                      .ToListAsync();


            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion

            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(zadatci));

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zadatak.IdZadatak));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(1.5f);
                    column.HeaderCell("ID Zadatka");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zadatak.Vrsta));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Naziv");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zadatak.Oibnositelj));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(2);
                    column.HeaderCell("OIB nositelja");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zadatak.IdStatusNavigation.NazivStatus));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(2);
                    column.HeaderCell("Status");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zadatak.VrPoc));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(2);
                    column.HeaderCell("Vrijeme početka");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zadatak.VrKraj));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(2);
                    column.HeaderCell("Vrijeme kraja");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Zadatak.VrKrajOcekivano));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(7);
                    column.Width(2);
                    column.HeaderCell("Očekivano vrijeme kraja");
                });
            });

            #endregion

            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=zahtjevi.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }


        //pocetakNiko
        public async Task<IActionResult> PartnerExcel()
        {
            var partneri = await ctx.Partner
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdPartner)
                                  .ToListAsync();

            var vrste = await ctx.Partner.AsNoTracking().OrderBy(d => d.IdPartner)
                                  .Select(d => d.IdTipPartneraNavigation.TipPartnera1)
                                  .ToListAsync();

            var projekti = await ctx.Partner.AsNoTracking().OrderBy(d => d.IdPartner)
                                  .Select(d => d.IdTipPartneraNavigation.TipPartnera1)
                                  .ToListAsync();

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis partnera";
                excel.Workbook.Properties.Author = "RPPP05";
                var worksheet = excel.Workbook.Worksheets.Add("Partneri");

                //First add the headers
                worksheet.Cells[1, 1].Value = "ID partnera";
                worksheet.Cells[1, 2].Value = "ID Projekta";
                worksheet.Cells[1, 3].Value = "Naziv Tipa";
                worksheet.Cells[1, 4].Value = "OIB";
                worksheet.Cells[1, 5].Value = "Adresa";
                worksheet.Cells[1, 6].Value = "IBAN";
                worksheet.Cells[1, 7].Value = "Email";
                worksheet.Cells[1, 8].Value = "Naziv partnera";


                for (int i = 0; i < partneri.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = partneri[i].IdPartner;
                    worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i + 2, 2].Value = projekti[i];
                    worksheet.Cells[i + 2, 3].Value = vrste[i];
                    worksheet.Cells[i + 2, 4].Value = partneri[i].Oib;
                    worksheet.Cells[i + 2, 5].Value = partneri[i].AdresaPartner;
                    worksheet.Cells[i + 2, 6].Value = partneri[i].Ibanpartner;
                    worksheet.Cells[i + 2, 7].Value = partneri[i].EmailPartner;
                    worksheet.Cells[i + 2, 8].Value = partneri[i].NazivPartner;

                }

                worksheet.Cells[1, 1, partneri.Count + 1, 8].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "Partneri.xlsx");
        }
        public async Task<IActionResult> SuradnikDetailExcel()
        {
            var suradnici = await ctx.Suradnik
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdSuradnik)
                                  .ToListAsync();

            var kvalifikacije = await ctx.Suradnik.AsNoTracking().OrderBy(d => d.IdPartner)
                                  .Select(d => d.IdKvalifikacijaNavigation.NazivKvalifikacija)
                                  .ToListAsync();

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis Suradnika";
                excel.Workbook.Properties.Author = "RPPP05";
                var worksheet = excel.Workbook.Worksheets.Add("Suradnici");

                //First add the headers
                worksheet.Cells[1, 1].Value = "ID suradnika";
                worksheet.Cells[1, 2].Value = "OIB";
                worksheet.Cells[1, 3].Value = "Broj mobitela";
                worksheet.Cells[1, 4].Value = "Kvalifikacija";
                worksheet.Cells[1, 5].Value = "Ime";
                worksheet.Cells[1, 6].Value = "Prezime";
                worksheet.Cells[1, 7].Value = "Email";
                worksheet.Cells[1, 8].Value = "Stranka";

                for (int i = 0; i < suradnici.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = suradnici[i].IdSuradnik;
                    worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i + 2, 2].Value = suradnici[i].Oib;
                    worksheet.Cells[i + 2, 3].Value = suradnici[i].Mobitel;
                    worksheet.Cells[i + 2, 4].Value = kvalifikacije[i];
                    worksheet.Cells[i + 2, 5].Value = suradnici[i].Ime;
                    worksheet.Cells[i + 2, 6].Value = suradnici[i].Prezime;
                    worksheet.Cells[i + 2, 7].Value = suradnici[i].Mail;
                    worksheet.Cells[i + 2, 8].Value = suradnici[i].Stranka;
                }

                worksheet.Cells[1, 1, suradnici.Count + 1, 7].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "Suradnici.xlsx");
        }
        public async Task<IActionResult> PartneriSuradniciExcel()
        {
            var partneri = await ctx.Partner
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdPartner)
                                  .ToListAsync();
            var vrste = await ctx.Partner
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdPartner)
                                  .Select(d => d.IdTipPartneraNavigation.TipPartnera1)
                                  .ToListAsync();
            var projekti = await ctx.Partner
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdPartner)
                                  .Select(d => d.IdTipPartneraNavigation.TipPartnera1)
                                  .ToListAsync();


            var suradnicisvi = await ctx.Suradnik
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdSuradnik)
                                  .ToListAsync();
            byte[] content;

            using (ExcelPackage excel = new ExcelPackage())
            {
                for (int i = 0; i < partneri.Count; i++)
                {
                    var partner = partneri[i];

                    // Create a worksheet for each kartica
                    var worksheet = excel.Workbook.Worksheets.Add($"Partner_{i + 1}");


                    worksheet.Cells[1, 1].Value = "ID partnera";
                    worksheet.Cells[1, 2].Value = "ID Projekta";
                    worksheet.Cells[1, 3].Value = "Naziv Tipa";
                    worksheet.Cells[1, 4].Value = "OIB";
                    worksheet.Cells[1, 5].Value = "Adresa";
                    worksheet.Cells[1, 6].Value = "IBAN";
                    worksheet.Cells[1, 7].Value = "Email";
                    worksheet.Cells[1, 8].Value = "Naziv Partnera";


                    worksheet.Cells[2, 1].Value = partneri[i].IdPartner;
                    worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[2, 2].Value = projekti[i];
                    worksheet.Cells[2, 3].Value = vrste[i];
                    worksheet.Cells[2, 4].Value = partneri[i].Oib;
                    worksheet.Cells[2, 5].Value = partneri[i].AdresaPartner;
                    worksheet.Cells[2, 6].Value = partneri[i].Ibanpartner;
                    worksheet.Cells[2, 7].Value = partneri[i].EmailPartner;
                    worksheet.Cells[2, 8].Value = partneri[i].NazivPartner;



                    // Add headers to the worksheet
                    worksheet.Cells[4, 1].Value = "ID suradnika";
                    worksheet.Cells[4, 2].Value = "OIB";
                    worksheet.Cells[4, 3].Value = "Broj mobitela";
                    worksheet.Cells[4, 4].Value = "Kvalifikacija";
                    worksheet.Cells[4, 5].Value = "Ime";
                    worksheet.Cells[4, 6].Value = "Prezime";
                    worksheet.Cells[4, 7].Value = "Email";
                    worksheet.Cells[4, 8].Value = "Stranka";

                    // Filter transakcije for the current kartica
                    var suradnici = suradnicisvi.Where(t => t.IdPartner == partner.IdPartner).ToList();

                    for (int j = 0; j < suradnici.Count; j++)
                    {

                        var kvalifikacije = ctx.Suradnik.AsNoTracking().Where(d => d.IdSuradnik == suradnici[j].IdSuradnik)
                                                            .Select(d => d.IdKvalifikacijaNavigation.NazivKvalifikacija).FirstOrDefault();


                        worksheet.Cells[j + 5, 1].Value = suradnici[j].IdSuradnik;
                        worksheet.Cells[j + 5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[j + 5, 2].Value = suradnici[j].Oib;
                        worksheet.Cells[j + 5, 3].Value = suradnici[j].Mobitel;
                        worksheet.Cells[j + 5, 4].Value = kvalifikacije;
                        worksheet.Cells[j + 5, 5].Value = suradnici[j].Ime;
                        worksheet.Cells[j + 5, 6].Value = suradnici[j].Prezime;
                        worksheet.Cells[j + 5, 7].Value = suradnici[j].Mail;
                        worksheet.Cells[j + 5, 8].Value = suradnici[j].Stranka;
                    }

                    worksheet.Cells[1, 1, suradnici.Count + 1, 7].AutoFitColumns();

                }
                content = excel.GetAsByteArray();
                return File(content, ExcelContentType, "master(Partner)-detail(Suradnik).xlsx");
            }
        }

        public async Task<IActionResult> PartnerPDF()
        {
            string naslov = "Popis partnera";

            var suradnici = await ctx.Partner
                                      .AsNoTracking()
                                      .Select(s => new PartnerPomocniViewModel
                                      {
                                          IdPartner = s.IdPartner,
                                          Oib = s.Oib,
                                          AdresaPartner = s.AdresaPartner,
                                          Ibanpartner = s.Ibanpartner,
                                          EmailPartner = s.EmailPartner,
                                          NazivPartner = s.NazivPartner,
                                          IdProjekt = s.IdProjekt,
                                          IdTipPartnera = s.IdTipPartnera,
                                          TipPartnera1 = s.IdTipPartneraNavigation.TipPartnera1
                                      })
                                      .ToListAsync();


            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion

            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(suradnici));

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Partner.IdPartner));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("ID Partnera");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Partner.IdProjekt));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(false);
                    column.Order(2);
                    column.Width(1);
                    column.HeaderCell("ID Projekta");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Partner.IdTipPartneraNavigation.TipPartnera1));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(3);
                    column.HeaderCell("Tip partnera");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Partner.Oib));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(3);
                    column.HeaderCell("OIB");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Partner.AdresaPartner));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(2);
                    column.HeaderCell("Adresa");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Partner.Ibanpartner));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(2);
                    column.HeaderCell("IBAN");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Partner.EmailPartner));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(7);
                    column.Width(2);
                    column.HeaderCell("Email");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Partner.NazivPartner));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(8);
                    column.Width(2);
                    column.HeaderCell("Naziv partnera");
                });
            });

            #endregion

            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=partneri.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> SuradnikDetailPDF()
        {
            string naslov = "Popis suradnika";

            var suradnici = await ctx.Suradnik
                      .OrderBy(s => s.IdSuradnik)
                      .Select(s => new SuradnikDetailPomocniViewModel
                      {
                          IdSuradnik = s.IdSuradnik,
                          Oib = s.Oib,
                          Mobitel = s.Mobitel,
                          Ime = s.Ime,
                          Prezime = s.Prezime,
                          IdKvalifikacija = s.IdKvalifikacija,
                          IdPartner = s.IdPartner,
                          Mail = s.Mail,
                          Stranka = s.Stranka,
                          NazivKvalifikacija = s.IdKvalifikacijaNavigation.NazivKvalifikacija
                      })
                      .ToListAsync();


            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion

            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(suradnici));

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.IdSuradnik));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(1.5f);
                    column.HeaderCell("ID Suradnika");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.Oib));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("OIB");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.Mobitel));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(2);
                    column.HeaderCell("Broj mobitela");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.IdKvalifikacijaNavigation.NazivKvalifikacija));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(2);
                    column.HeaderCell("Kvalifikacija");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.Ime));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(2);
                    column.HeaderCell("Ime");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.Prezime));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(2);
                    column.HeaderCell("Prezime");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.Mail));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(7);
                    column.Width(2);
                    column.HeaderCell("Email");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.Stranka));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(7);
                    column.Width(2);
                    column.HeaderCell("Stranka");
                });
            });

            #endregion

            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=suradnici.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }
        //Nina
        public async Task<IActionResult> SuradnikExcel()
        {
            var suradnici = await ctx.Suradnik
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdSuradnik)
                                  .ToListAsync();

            var kvalifikacije = await ctx.Suradnik.AsNoTracking().OrderBy(d => d.IdSuradnik)
                                  .Select(d => d.IdKvalifikacijaNavigation.NazivKvalifikacija)
                                  .ToListAsync();

            var partneri = await ctx.Suradnik.AsNoTracking().OrderBy(d => d.IdSuradnik)
                                  .Select(d => d.IdPartnerNavigation.NazivPartner)
                                  .ToListAsync();

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis suradnika";
                excel.Workbook.Properties.Author = "RPPP05";
                var worksheet = excel.Workbook.Worksheets.Add("Suradnici");

                //First add the headers
                worksheet.Cells[1, 1].Value = "OIB";
                worksheet.Cells[1, 2].Value = "Broj mobitela";
                worksheet.Cells[1, 3].Value = "Ime";
                worksheet.Cells[1, 4].Value = "Prezime";
                worksheet.Cells[1, 5].Value = "Email";
                worksheet.Cells[1, 6].Value = "Stranka";
                worksheet.Cells[1, 7].Value = "Kvalifikacija";
                worksheet.Cells[1, 8].Value = "Naziv partnera";


                for (int i = 0; i < suradnici.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = suradnici[i].Oib;
                    worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i + 2, 2].Value = suradnici[i].Mobitel;
                    worksheet.Cells[i + 2, 3].Value = suradnici[i].Ime;
                    worksheet.Cells[i + 2, 4].Value = suradnici[i].Prezime;
                    worksheet.Cells[i + 2, 5].Value = suradnici[i].Mail;
                    worksheet.Cells[i + 2, 6].Value = suradnici[i].Stranka;
                    worksheet.Cells[i + 2, 7].Value = kvalifikacije[i];
                    worksheet.Cells[i + 2, 8].Value = partneri[i];

                }

                worksheet.Cells[1, 1, suradnici.Count + 1, 8].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "Suradnici.xlsx");
        }
        public async Task<IActionResult> PosaoExcel()
        {
            var poslovi = await ctx.Posao
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdPosao)
                                  .ToListAsync();

            var vrste = await ctx.Posao.AsNoTracking().OrderBy(d => d.IdPosao)
                                  .Select(d => d.IdVrstaPosaoNavigation.NazivPosao)
                                  .ToListAsync();

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis Poslova";
                excel.Workbook.Properties.Author = "RPPP05";
                var worksheet = excel.Workbook.Worksheets.Add("Poslovi");

                //First add the headers
                worksheet.Cells[1, 1].Value = "ID posla";
                worksheet.Cells[1, 2].Value = "Opis";
                worksheet.Cells[1, 3].Value = "Predvideno vrijeme trajanja u danima";
                worksheet.Cells[1, 4].Value = "Vrsta posla";

                for (int i = 0; i < poslovi.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = poslovi[i].IdPosao;
                    worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i + 2, 2].Value = poslovi[i].Opis;
                    worksheet.Cells[i + 2, 3].Value = poslovi[i].PredVrTrajanjaDani;
                    worksheet.Cells[i + 2, 4].Value = vrste[i];
                }

                worksheet.Cells[1, 1, poslovi.Count + 1, 7].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "Poslovi.xlsx");
        }
        public async Task<IActionResult> SuradniciPosloviExcel()
        {

            var kvalifikacije = await ctx.Suradnik
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdSuradnik)
                                  .Select(d => d.IdKvalifikacijaNavigation.NazivKvalifikacija)
                                  .ToListAsync();
            var partneri = await ctx.Suradnik
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdSuradnik)
                                  .Select(d => d.IdPartnerNavigation.NazivPartner)
                                  .ToListAsync();


            var poslovisvi = await ctx.Radi
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdPosao)
                                  .ToListAsync();
            var suradnici = await ctx.Suradnik
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdSuradnik)
                                  .ToListAsync();
            byte[] content;

            using (ExcelPackage excel = new ExcelPackage())
            {
                for (int i = 0; i < suradnici.Count; i++)
                {
                    var suradnik = suradnici[i];

                    // Create a worksheet for each kartica
                    var worksheet = excel.Workbook.Worksheets.Add($"Suradnik_{i + 1}");


                    worksheet.Cells[1, 1].Value = "OIB";
                    worksheet.Cells[1, 2].Value = "Broj mobitela";
                    worksheet.Cells[1, 3].Value = "Ime";
                    worksheet.Cells[1, 4].Value = "Prezime";
                    worksheet.Cells[1, 5].Value = "Email";
                    worksheet.Cells[1, 6].Value = "Stranka";
                    worksheet.Cells[1, 7].Value = "Kvalifikacija";
                    worksheet.Cells[1, 8].Value = "Naziv partnera";


                    worksheet.Cells[2, 1].Value = suradnici[i].Oib;
                    worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[2, 2].Value = suradnici[i].Mobitel;
                    worksheet.Cells[2, 3].Value = suradnici[i].Ime;
                    worksheet.Cells[2, 4].Value = suradnici[i].Prezime;
                    worksheet.Cells[2, 5].Value = suradnici[i].Mail;
                    worksheet.Cells[2, 6].Value = suradnici[i].Stranka;
                    worksheet.Cells[2, 7].Value = kvalifikacije[i];
                    worksheet.Cells[2, 8].Value = partneri[i];



                    // Add headers to the worksheet
                    worksheet.Cells[4, 1].Value = "ID posla";
                    worksheet.Cells[4, 2].Value = "Vrsta posla";

                    // Filter transakcije for the current kartica
                    var poslovi = poslovisvi.Where(t => t.IdSuradnik == suradnik.IdSuradnik).ToList();

                    for (int j = 0; j < poslovi.Count; j++)
                    {

                        var vrste = ctx.Posao.AsNoTracking().Where(d => d.IdPosao == poslovi[j].IdPosao)
                                                            .Select(d => d.IdVrstaPosaoNavigation.NazivPosao).FirstOrDefault();


                        worksheet.Cells[j + 5, 1].Value = poslovi[j].IdPosao;
                        worksheet.Cells[j + 5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[j + 5, 2].Value = vrste[j];
                    }

                    worksheet.Cells[1, 1, poslovi.Count + 1, 7].AutoFitColumns();

                }
                content = excel.GetAsByteArray();
                return File(content, ExcelContentType, "master(Suradnik)-detail(Posao).xlsx");
            }
        }
        public async Task<IActionResult> SuradnikPDF()
        {
            string naslov = "Popis suradnika";

            var poslovi = await ctx.Suradnik
                                      .AsNoTracking()
                                      .Select(s => new SuradnikPomocniViewModel
                                      {
                                          IdSuradnik = s.IdSuradnik,
                                          IdPartner = s.IdPartner,
                                          Oib = s.Oib,
                                          Ime = s.Ime,
                                          Prezime = s.Prezime,
                                          Mail = s.Mail,
                                          Mobitel = s.Mobitel,
                                          Stranka = s.Stranka,
                                          NazivKvalifikacija = s.IdKvalifikacijaNavigation.NazivKvalifikacija,
                                      })
                                      .ToListAsync();


            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion

            #region Postavljanje izvora podataka i stupaca

            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(poslovi));

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);

                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });

                columns.AddColumn(column =>
                {

                    column.PropertyName(nameof(Suradnik.IdSuradnik));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("ID Suradnika");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.IdPartner));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(false);
                    column.Order(2);
                    column.Width(1);
                    column.HeaderCell("ID Partnera");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.Ime));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(3);
                    column.HeaderCell("Ime suradnika");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.Prezime));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(3);
                    column.HeaderCell("Prezime suradnika");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.Mail));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(2);
                    column.HeaderCell("Email");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.Mobitel));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(2);
                    column.HeaderCell("Broj mobitela");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.Stranka));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(7);
                    column.Width(2);
                    column.HeaderCell("Stranka");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Suradnik.IdKvalifikacijaNavigation.NazivKvalifikacija));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(8);
                    column.Width(2);
                    column.HeaderCell("Kvalifikacija");
                });
            });


            #endregion

            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=suradnici.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }
        public async Task<IActionResult> PosaoPDF()
        {
            string naslov = "Popis poslova";

            var poslovi = await ctx.Posao
                      .OrderBy(s => s.IdPosao)
                      .Select(s => new PosaoPomocniViewModel
                      {
                          IdPosao = s.IdPosao,
                          NazivPosao = s.IdVrstaPosaoNavigation.NazivPosao,
                          Opis = s.Opis,
                          PredVrTrajanjaDani = s.PredVrTrajanjaDani
                      })
                      .ToListAsync();
            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion

            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(poslovi));


            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Posao.IdPosao));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(1.5f);
                    column.HeaderCell("ID posla");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Posao.IdVrstaPosaoNavigation.NazivPosao));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Naziv posla");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Posao.Opis));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(4);
                    column.HeaderCell("Opis posla");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Posao.PredVrTrajanjaDani));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(3);
                    column.HeaderCell("Predvideno vrijeme trajanja u danima");
                });
            });
            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=poslovi.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> ProjektExcel()
        {
            var projekti = await ctx.Projekt
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdProjekt)
                                  .ToListAsync();

            var vrste = await ctx.Projekt.AsNoTracking().OrderBy(d => d.IdProjekt)
                                  .Select(d => d.IdTipNavigation.NazivTip)
                                  .ToListAsync();

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis projekata";
                excel.Workbook.Properties.Author = "RPPP05";
                var worksheet = excel.Workbook.Worksheets.Add("Projekti");

                //First add the headers
                worksheet.Cells[1, 1].Value = "ID Projekta";
                worksheet.Cells[1, 2].Value = "Vrsta projekta";
                worksheet.Cells[1, 3].Value = "Opis";
                worksheet.Cells[1, 4].Value = "Vrijeme pocetka";
                worksheet.Cells[1, 5].Value = "Vrijeme Kraja";
                worksheet.Cells[1, 6].Value = "Naziv";



                for (int i = 0; i < projekti.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = projekti[i].IdProjekt;
                    worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i + 2, 2].Value = vrste[i];
                    worksheet.Cells[i + 2, 3].Value = projekti[i].Opis;
                    worksheet.Cells[i + 2, 4].Value = projekti[i].VrPocetak.ToString("g");
                    worksheet.Cells[i + 2, 5].Value = projekti[i].VrKraj.HasValue ? projekti[i].VrKraj.Value.ToString("g") : "";
                    worksheet.Cells[i + 2, 6].Value = projekti[i].Naziv;


                }

                worksheet.Cells[1, 1, projekti.Count + 1, 6].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "Projekti.xlsx");
        }

        public async Task<IActionResult> DokumentExcel()
        {
            var dokumenti = await ctx.Dokument
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdDokument)
                                  .ToListAsync();

            var vrste = await ctx.Dokument.AsNoTracking().OrderBy(d => d.IdDokument)
                                  .Select(d => d.IdVrstaDokNavigation.NazivVrstaDok)
                                  .ToListAsync();

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis Dokumenata";
                excel.Workbook.Properties.Author = "RPPP05";
                var worksheet = excel.Workbook.Worksheets.Add("Dokumenti");

                //First add the headers
                worksheet.Cells[1, 1].Value = "ID dokument";
                worksheet.Cells[1, 2].Value = "Ekstenzija";
                worksheet.Cells[1, 3].Value = "Velicina dokumenta";
                worksheet.Cells[1, 4].Value = "Pripadni projekt";
                worksheet.Cells[1, 5].Value = "Vrsta dokumenta";
                worksheet.Cells[1, 6].Value = "Naziv dokumenta";
                worksheet.Cells[1, 7].Value = "Lokacija dokumenta";

                for (int i = 0; i < dokumenti.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = dokumenti[i].IdDokument;
                    worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[i + 2, 2].Value = dokumenti[i].TipDokument;
                    worksheet.Cells[i + 2, 3].Value = dokumenti[i].VelicinaDokument;
                    worksheet.Cells[i + 2, 4].Value = dokumenti[i].IdProjekt;
                    worksheet.Cells[i + 2, 5].Value = vrste[i];
                    worksheet.Cells[i + 2, 6].Value = dokumenti[i].NazivDatoteka;
                    worksheet.Cells[i + 2, 7].Value = dokumenti[i].URLdokument;
                }

                worksheet.Cells[1, 1, dokumenti.Count + 1, 7].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "Dokumenti.xlsx");
        }


        public async Task<IActionResult> ProjektPDF()
        {
            string naslov = "Popis projekata";

            var projekti = await ctx.Projekt.AsNoTracking().Select(
                s => new ProjektPomocni
                {
                    IdProjekt = s.IdProjekt,
                    VrPocetak = s.VrPocetak,
                    VrKraj = s.VrKraj,
                    Opis = s.Opis,
                    Naziv = s.Naziv,
                    IdTip = s.IdTip,
                    NazivTip = s.IdTipNavigation.NazivTip
                }).ToListAsync();
            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion

            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(projekti));

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Left);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Projekt.IdProjekt));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(1);
                    column.HeaderCell("ID Projekta");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Projekt.VrPocetak));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(2);
                    column.HeaderCell("Vrijeme pocetka projekta");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Projekt.VrKraj));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(2);
                    column.HeaderCell("Vrijeme kraja projekta");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Projekt.Opis));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(2);
                    column.HeaderCell("Opis");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Projekt.Naziv));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(2);
                    column.HeaderCell("Naziv");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Projekt.IdTipNavigation.NazivTip));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(2);
                    column.HeaderCell("Tip projekta");
                });
            });

            #endregion

            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=projekti.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }
        public async Task<IActionResult> DokumentPDF()
        {
            string naslov = "Popis dokumenata";

            var dokumenti = await ctx.Dokument.AsNoTracking().Select(
                               s => new DokPomViewModel
                               {
                                   IdDokument = s.IdDokument,
                                   IdProjekt = s.IdProjekt,
                                   TipDokument = s.TipDokument,
                                   VelicinaDokument = s.VelicinaDokument,
                                   IdVrstaDok = s.IdVrstaDok,
                                   NazivDatoteka = s.NazivDatoteka,
                                   URLdokument = s.URLdokument,
                                   NazivVrstaDok = s.IdVrstaDokNavigation.NazivVrstaDok,

                               }).ToListAsync();

            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion

            #region Postavljanje izvora podataka i stupaca

            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(dokumenti));

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);

                    column.CellsHorizontalAlignment(HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Left);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Dokument.IdDokument));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(1);
                    column.HeaderCell("ID Dokumenta");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Dokument.IdProjekt));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(1);
                    column.HeaderCell("Pripadni projekt");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Dokument.TipDokument));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(1);
                    column.HeaderCell("Ekstenzija");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Dokument.VelicinaDokument));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(2);
                    column.HeaderCell("Velicina");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Dokument.NazivDatoteka));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(2);
                    column.HeaderCell("Naziv");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Dokument.URLdokument));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(4);
                    column.HeaderCell("Lokacija");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName(nameof(Dokument.IdVrstaDokNavigation.NazivVrstaDok));
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(7);
                    column.Width(2);
                    column.HeaderCell("Kategorija");
                });
            });

            #endregion

            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {

                Response.Headers.Add("content-disposition", "inline; filename=zahtjevi.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }
    }
}
    