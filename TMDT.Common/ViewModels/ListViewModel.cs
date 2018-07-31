using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMDT.Common.ViewModels
{
    public class ListViewModel
    {
        public string Name { set; get; }
        public int Quantity { set; get; }
        public bool Status { set; get; }
        public int ColorId { set; get; }
        public int SizeId { set; get; }
    }
}
