using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMDT.Model.Models
{
    [Table("CommentVotes")]
    public class CommentVote
    {

        [Key]
        [Column(Order = 1)]
        public int CommentId { set; get; }
        [Key]
        [Column(TypeName = "nvarchar", Order = 2)]
        public string UserId { set; get; }
        [ForeignKey("CommentId")]
        public virtual Comment Comment { set; get; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { set; get; }
    }
}
