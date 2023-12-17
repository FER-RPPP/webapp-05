using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class PartnerSort
    {
        public static IQueryable<Partner> ApplySort(this IQueryable<Partner> query, int sort, bool ascending)
        {
            Expression<Func<Partner, object>> orderSelector = sort switch
            {
                1 => d => d.IdPartner,
                2 => d => d.NazivPartner,
                3 => d => d.AdresaPartner,
                4 => d => d.EmailPartner,
                5 => d => d.Oib,
                6 => d => d.Ibanpartner,
                7 => d => d.IdTipPartneraNavigation.TipPartnera1,
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
