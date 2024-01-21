using RPPP_WebApp.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace RPPP_WebApp.Exstensions.Selectors
{
    /// <summary>
    /// Razred koji sadrzi ekstenziju za primjenu sortiranja na upit za transakciju
    /// </summary>
    public static class TransakcijaSort
    {
        /// <summary>
        /// Primjenjuje sortiranje na upit za transakciju
        /// </summary>
        /// <param name="query">Upit za transakciju</param>
        /// <param name="sort">Indeks prema kojem se pokrece sortiranje</param>
        /// <param name="ascending">True ako je sortiranje uzlazno, inače false</param>
        /// <returns>Sortirani upit za transakciju</returns>
        public static IQueryable<Transakcija> ApplySort(this IQueryable<Transakcija> query, int sort, bool ascending)
        {
            Expression<Func<Transakcija, object>> orderSelector = sort switch
            {
                1 => d => d.PrimateljIban,
                2 => d => d.SubjektIban,
                3 => d => d.IdTransakcije,
                4 => d => d.Vrsta,
                5 => d => d.Vrijednost,
                6 => d => d.Valuta,
                7 => d => d.Opis,
                8 => d => d.IdTransakcijeNavigation.NazivTransakcije,
                _ => null
            };

            if (orderSelector != null)
            {
                query = ascending ?
                       query.OrderBy(orderSelector) :
                       query.OrderByDescending(orderSelector);
            }

            return query;
        }
    }
}
