using RPPP_WebApp.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace RPPP_WebApp.Exstensions.Selectors
{
    /// <summary>
    /// Razred koji sadrzi ekstenziju za primjenu sortiranja na upit za projektnu karticu
    /// </summary>
    public static class ProjektnaKarticaSort
    {
        /// <summary>
        /// Primjenjuje sortiranje na upit za projektnu karticu
        /// </summary>
        /// <param name="query">Upit za projektnu karticu</param>
        /// <param name="sort">Indeks prema kojem se pokrece sortiranje</param>
        /// <param name="ascending">True ako je sortiranje uzlazno, inače false</param>
        /// <returns>Sortirani upit za projektnu karticu</returns>
        public static IQueryable<ProjektnaKartica> ApplySort(this IQueryable<ProjektnaKartica> query, int sort, bool ascending)
        {
            Expression<Func<ProjektnaKartica, object>> orderSelector = sort switch
            {
                1 => d => d.SubjektIban,
                2 => d => d.Saldo,
                3 => d => d.Valuta,
                4 => d => d.IdProjekt,
                5 => d => d.VrijemeOtvaranja,
                6 => d => d.IdProjektNavigation.Naziv,
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
