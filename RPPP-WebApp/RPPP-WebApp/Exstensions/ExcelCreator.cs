using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using RPPP_WebApp.Util;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RPPP_WebApp.Exstensions
{
    /// <summary>
    /// Razred koji sadrzi ekstenziju za stvaranje Excel iz baze podataka
    /// </summary>
    public static class ExcelCreator
    {
        /// <summary>
        /// Stvara Exceliz baze podataka
        /// </summary>
        /// <typeparam name="T">Tip podataka u kolekciji</typeparam>
        /// <param name="data">Kolekcija podataka</param>
        /// <param name="worksheetName">Ime radnog lista</param>
        /// <returns>Excel datoteka</returns>
        public static ExcelPackage CreateExcel<T>(this IEnumerable<T> data, string worksheetName)
        {
            ExcelPackage excel = new ExcelPackage();
            var worksheet = excel.Workbook.Worksheets.Add(worksheetName);
            int row = 1;
            int col = 1;

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var prop in props)
            {
                if (prop.PropertyType is IEnumerable)
                    continue; //preskoči kolekcije
                string name = prop.Name;
                if (prop.IsDefined(typeof(DisplayAttribute)))
                {
                    name = prop.GetCustomAttribute<DisplayAttribute>().Name;
                }
                worksheet.Cells[row, col++].Value = name;
            }


            foreach (T t in data)
            {
                ++row;
                col = 1;
                foreach (var prop in props)
                {
                    if (prop.PropertyType is IEnumerable)
                        continue; //preskoči kolekcije

                    object value = prop.GetValue(t);
                    worksheet.Cells[row, col].Value = value;
                    if (prop.IsDefined(typeof(ExcelFormatAttribute)))
                    {
                        string format = prop.GetCustomAttribute<ExcelFormatAttribute>().ExcelFormat;
                        if (!string.IsNullOrWhiteSpace(format))
                        {
                            worksheet.Cells[row, col].Style.Numberformat.Format = format;
                        }
                    }
                    ++col;
                }
            }

            worksheet.Cells[1, 1, row - 1, col - 1].AutoFitColumns();
            return excel;
        }
    }
}
