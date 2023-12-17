
using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class ZahtjevSort
    {
        public static IQueryable<Zahtjev> ApplySort(this IQueryable<Zahtjev> query, int sort, bool ascending)
        {
            Expression<Func<Zahtjev, object>> orderSelector = sort switch
            {
                1 => d => d.IdZahtjev,
                2 => d => d.Prioritet,
                3 => d => d.Opis,
                4 => d => d.VrPocetak,
                5 => d => d.VrKraj,
                6 => d => d.IdVrstaNavigation.NazivVrsta,
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