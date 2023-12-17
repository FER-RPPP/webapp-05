using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class DokumentSort
    {
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