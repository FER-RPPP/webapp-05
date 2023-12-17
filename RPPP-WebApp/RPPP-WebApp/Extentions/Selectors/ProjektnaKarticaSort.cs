
using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class ProjektnaKarticaSort
    {
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