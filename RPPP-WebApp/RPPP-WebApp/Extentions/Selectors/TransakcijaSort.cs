
using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class TransakcijaSort
    {
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
