using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    /// <summary>
    /// razred za sortiranje dokumenata
    /// </summary>
    public static class DokumentSort
    {
        /// <summary>
        /// funkcija za sortiranje dokumenata
        /// </summary>
        /// <param name="query">upit za dokument</param>
        /// <param name="sort">element po kojem se sortira</param>
        /// <param name="ascending">smjer sortiranja</param>
        /// <returns>vraca sortiran upit</returns>
        public static IQueryable<Dokument> ApplySort(this IQueryable<Dokument> query, int sort, bool ascending)
        {
            Expression<Func<Dokument, object>> orderSelector = sort switch
            {
                1 => p => p.IdDokument,
                2 => p => p.NazivDatoteka,
                3 => p => p.TipDokument,
                4 => p => p.VelicinaDokument,
                5 => p => p.IdVrstaDokNavigation.NazivVrstaDok,
                6 => p => p.URLdokument,
                7 => p => p.IdProjektNavigation.Naziv,
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