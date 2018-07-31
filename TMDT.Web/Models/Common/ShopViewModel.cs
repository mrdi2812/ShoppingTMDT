using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.Web.Models.Common
{
    public class ShopViewModel
    {
        public IEnumerable<ProductCategoryViewModel> ProductCategory { set; get; }
        public IEnumerable<ColorViewModel> Colors { set; get; }
        public IEnumerable<SizeViewModel> Sizes { get; set; }        
    }
}