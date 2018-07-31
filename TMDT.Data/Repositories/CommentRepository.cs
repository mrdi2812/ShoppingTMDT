using System.Collections.Generic;
using System.Linq;
using TMDT.Data.Infrastructure;
using TMDT.Model.Models;

namespace TMDT.Data.Repositories
{
    public interface ICommentRepository :IRepository<Comment>
    {
        IEnumerable<AppUser> GetListUserByProductId(int productId);
        IEnumerable<Comment> GetListCommentByProductId(int productId);
        IEnumerable<AppUser> GetListUserByPostId(int postId);
        IEnumerable<CommentVote> getListVote(int commentId);
        IEnumerable<Comment> GetListCommentByPostId(int postId);
        IEnumerable<AppUser> GetUserById(string userId);
    }
    public class CommentRepository : RepositoryBase<Comment>,ICommentRepository
    {
        public CommentRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<Comment> GetListCommentByPostId(int postId)
        {
            var query = from c in DbContext.Comments
                        join cv in DbContext.CommentVotes
                        on c.ID equals cv.CommentId
                        where c.PostId == postId
                        select c;
            return query;
        }

        public IEnumerable<Comment> GetListCommentByProductId(int productId)
        {
            var query = from c in DbContext.Comments
                        join cv in DbContext.CommentVotes
                        on c.ID equals cv.CommentId
                        where c.ProductId == productId
                        select c;
            return query;
        }

        public IEnumerable<AppUser> GetListUserByPostId(int postId)
        {
            var query = from c in DbContext.Comments
                        where c.PostId == postId
                        select c.AppUser;
            return query;
        }

        public IEnumerable<AppUser> GetListUserByProductId(int productId)
        {
            var query = from c in DbContext.Comments
                        where c.ProductId == productId
                        select c.AppUser;
            return query;
        }

        public IEnumerable<CommentVote> getListVote(int commentId)
        {
            var query = from c in DbContext.CommentVotes
                        where c.CommentId == commentId
                        select c;
            return query;
        }

        public IEnumerable<AppUser> GetUserById(string userId)
        {
            var query = from c in DbContext.Comments
                        where c.UserId == userId
                        select c.AppUser;
            return query;
        }
    }
}
