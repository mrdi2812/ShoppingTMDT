using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.Web.Models
{
    public class CommentViewModel
    {
        public int ID { set; get; }
        public string Content { set; get; }
        public string ParentId { set; get; }
        public string AttachImage { set; get; }
        public DateTime CreateDate { set; get; }
        public string CreateBy { set; get; }
        public DateTime ModifiedDate { set; get; }
        public string Pings { set; get; }
        public int ReplyCount { set; get; }
        public bool Status { set; get; }
        public string UserId { set; get; }
        public int? PostId { set; get; }
        public int? ProductId { set; get; } 
        public virtual ApplicationUserViewModel AppUser { set; get; }
        public ICollection<CommentVoteViewModel> CommentVotes { set; get; }
        public virtual PostViewModel Post { set; get; }
        public virtual ProductViewModel Product { set; get; }
    }
}