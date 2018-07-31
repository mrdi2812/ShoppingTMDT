using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMDT.Model.Models
{
    [Table("Comments")]
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { set; get; }

        [StringLength(500)]
        public string AttachImage { set; get; }

        public DateTime CreateDate { set; get; }

        [StringLength(250)]
        public string CreateBy { set; get; }

        public DateTime? ModifiedDate { set; get; }
        [MaxLength(500)]
        public string Content { set; get; }
        public string ParentId { set; get; }
        public int ReplyCount { set; get; }
        [StringLength(500)]
        public string Pings { set; get; }

        public bool Status { set; get; }
        public int? PostId { set; get; }
        public int? ProductId { set; get; }

        [StringLength(128)]
        [Column(TypeName = "nvarchar")]
        public string UserId { set; get; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { set; get; }
        public virtual ICollection<CommentVote> CommentVotes { set; get; }
       
    }
}
