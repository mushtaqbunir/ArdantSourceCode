using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Data.ModelVm
{
    public class PageListData<T>
    {

        public int totalPageQuantity { get; set; } = 0;
        public int totalCount { get; set; } = 0;
        public List<T> ListOfData { get; set; } = new();
        public IQueryable<T> QueryableOfData { get; set; } 
    }
}
