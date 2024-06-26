﻿
using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    /// <summary>
    /// razred za sortiranje projekata
    /// </summary>
    public static class ProjektSort
    {
        /// <summary>
        /// funkcija za sortiranje projekata
        /// </summary>
        /// <param name="query">upit za projekte</param>
        /// <param name="sort">po kojem se elementu sortira</param>
        /// <param name="ascending">u kojem smjeru se sortira</param>
        /// <returns>sortiran upit</returns>
        public static IQueryable<Projekt> ApplySort(this IQueryable<Projekt> query, int sort, bool ascending)
        {
            Expression<Func<Projekt, object>> orderSelector = sort switch
            {
                1 => p => p.IdProjekt,
                2 => p => p.Naziv,
                3 => p => p.VrPocetak,
                4 => p => p.VrKraj,
                5 => p => p.IdTipNavigation.NazivTip,
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