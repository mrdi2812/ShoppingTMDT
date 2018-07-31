using System;

namespace TMDT.Web.Models.Common
{
    [Serializable]
    public class ShoppingCartViewModel
    {
        public int ProductId { set; get; }
        public ProductViewModel Product { set; get; }
        public string Category { set; get; }
        public int SizeId { set; get; }
        public int ColorId { set; get; }
        public int Quantity { set; get; }
    }
}