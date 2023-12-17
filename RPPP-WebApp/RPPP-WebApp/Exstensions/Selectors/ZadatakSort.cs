
using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class ZadatakSort
    {
        public static IQueryable<Zadatak> ApplySort(this IQueryable<Zadatak> query, int sort, bool ascending)
        {
            Expression<Func<Zadatak, object>> orderSelector = sort switch
            {
                1 => d => d.IdZadatak,
                2 => d => d.Vrsta,
                3 => d => d.Oibnositelj,
                4 => d => d.VrPoc,
                5 => d => d.VrKraj,
                6 => d => d.IdStatusNavigation.NazivStatus,
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
