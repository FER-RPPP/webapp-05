using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RPPP_WebApp.Models;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.Exstensions;
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
//using OfficeOpenXml;

namespace RPPP_WebApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly RPPP05Context ctx;
        private readonly IWebHostEnvironment environment;
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public ReportController(RPPP05Context ctx, IWebHostEnvironment environment)
        {
            this.ctx = ctx;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Export u Excel Projektne kartice
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
                    else {
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

        public async Task<IActionResult> ProjektnaKarticaTransakcijeExcel()
        {
            var kartice = await ctx.ProjektnaKartica
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdProjekt)
                                  .ToListAsync();
            var kartice2 = await ctx.ProjektnaKartica
                                  .AsNoTracking()
                                  .OrderBy(d => d.IdProjekt)
                                  .Select(d=> d.IdProjektNavigation.Naziv)
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
                            Saldo = (double) worksheet.Cells[row, 2].Value,
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
        public async Task<IActionResult> ProjektnaKarticaTransakcijePDF()
        {
            /*var kartice = await ctx.ProjektnaKartica
                                  .Select(u => new ProjektnaKarticaDenorm
                                  {
                                      subjektIBAN = u.SubjektIban,
                                      Saldo = (int)u.Saldo,
                                      valuta = u.Valuta,
                                      vrijemeOtvaranja = u.VrijemeOtvaranja,
                                      //idProjekt = u.IdProjektNavigation.Naziv,
                                      idProjekt = u.IdProjekt
                                      
                                  })
                                  .OrderBy(u => u.subjektIBAN)
                                  .ToListAsync();*/
            var transakcije =  await ctx.Transakcija
                                  .Select(u => new ProjektnaKarticaDenorm
                                  {
                                      primateljIBAN = u.PrimateljIban,
                                      subjektIBAN = u.SubjektIban,
                                      opis = u.Opis,
                                      valuta = u.Valuta,
                                      vrsta = u.Vrsta,
                                      vrijednost = u.Vrijednost

                                  }).ToListAsync();

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
        public class MasterDetailsHeaders : IPageHeader
        {
            private readonly string title;
            public MasterDetailsHeaders(string title)
            {
                this.title = title;
            }
            public IPdfFont PdfRptFont { set; get; }

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

    }



    
}
