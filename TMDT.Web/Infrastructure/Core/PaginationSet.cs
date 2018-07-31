using System.Collections.Generic;
using System.Linq;

namespace TMDT.Web.Infrastructure.Core
{
    public class PaginationSet<T>
    {
        public int Count
        {
            get
            {
                return (Items != null) ? Items.Count() : 0;
            }
        }
        public int PageIndex { set; get; }
        public int MaxPage { set; get; }
        public int PageSize { get; set; }
        public int TotalPages { set; get; }
        public int TotalRows { set; get; }
        public IEnumerable<T> Items { set; get; }
    }
}