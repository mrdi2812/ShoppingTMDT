using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TMDT.Web.Models.Common
{
    public class MenuViewModel
    {
        public MenuViewModel()
        {

        }
        public string ID { set; get; }

        [Required]
        [MaxLength(50)]
        public string Name { set; get; }

        [Required]
        [MaxLength(256)]
        public string URL { set; get; }

        public int DisplayOrder { set; get; }

        public string ParentId { set; get; }

        public MenuViewModel Parent { set; get; }

        public ICollection<MenuViewModel> ChildMenus { set; get; }

        public bool Status { set; get; }

        public string IconCss { get; set; }

        public string Class { set; get; }
    }
}