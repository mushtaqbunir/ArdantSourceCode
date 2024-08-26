using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Data.ModelVm
{
    public class GenericPaginationModel<T>
    {
        public List<T> List { get; set; }
        public List<T> ListSearch { get; set; }
        public IQueryable<T> ListSearchFilter { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; } = 1;
    }
}
