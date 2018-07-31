using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMDT.Model.Models
{
    [Table("AppRoles")]
    public class AppRole : IdentityRole
    {
        public AppRole() : base()
        {
        }

        public AppRole(string name, string description, string id)
        : this()
        {
            this.Name = name;
            this.Description = description;
            this.Id = id;
        }

        public virtual string Description { set; get; }
    }
}