using ArdantOffical.Data.ModelVm;
using System.Collections.Generic;
using System.Linq;

namespace ArdantOffical.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable,
             PaginationDTO pagination)
        {
            return queryable
                .Skip((pagination.Page - 1) * pagination.QuantityPerPage)
                .Take(pagination.QuantityPerPage);
        }

        public static List<T> PaginateList<T>(this List<T> queryable,
                    PaginationDTO pagination)
        {
            return queryable
                .Skip((pagination.Page - 1) * pagination.QuantityPerPage)
                .Take(pagination.QuantityPerPage).ToList();
        }

    }
}
