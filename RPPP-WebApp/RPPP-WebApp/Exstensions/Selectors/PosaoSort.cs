using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class PosaoSort
    {
        public static IQueryable<Posao> ApplySort(this IQueryable<Posao> query, int sort, bool ascending)
        {
            Expression<Func<Posao, object>> orderSelector = sort switch
            {
                1 => d => d.IdPosao,
                2 => d => d.Opis,
                3 => d => d.PredVrTrajanjaDani,
                4 => d => d.Uloga,
                5 => d => d.IdVrstaPosaoNavigation.NazivPosao,
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