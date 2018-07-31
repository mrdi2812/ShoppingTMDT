using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDT.Data.Infrastructure;
using TMDT.Model.Models;

namespace TMDT.Data.Repositories
{
    public interface ICommentVoteRepository :IRepository<CommentVote>
    {
        int Check(string userId, int commentId);
        void DeleteCommentVote(string userId, int commentId);
    }
    public class CommentVoteRepository : RepositoryBase<CommentVote>, ICommentVoteRepository
    {
        public CommentVoteRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public int Check(string userId, int commentId)
        {
            var query = from cv in DbContext.CommentVotes
                        where cv.CommentId == commentId &&
                        cv.UserId == userId
                        select cv;
            int total = query.Count();
            return total;
        }

        public void DeleteCommentVote(string userId, int commentId)
        {
            var query = DbContext.CommentVotes.SingleOrDefault(x => x.CommentId == commentId && x.UserId == userId);
            DbContext.CommentVotes.Remove(query);
        }
    }
}
