using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.Web.Models.Common
{
    public class ErrorViewModel
    {
        public int ID { set; get; }
        public string Messeage { set; get; }
        public string StackTrace { set; get; }
        public DateTime CreateDate { set; get; }
    }
}