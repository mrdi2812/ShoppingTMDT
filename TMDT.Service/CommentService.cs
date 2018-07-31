using System.Collections.Generic;
using System.Linq;
using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface ICommentService
    {
        Comment Add(Comment commnet);
        void Update(Comment commnet);
        Comment Delete(int id);
        void Save();
        Comment GetById(int id);
        IEnumerable<Comment> GetListCommentByPostId(int? postId);
        IEnumerable<AppUser> GetUserByPostId(int postId);
        IEnumerable<Comment> GetListCommentByProductId(int? productId);
        IEnumerable<AppUser> GetUserByProductId(int productId);
        IEnumerable<CommentVote> GetListCommentVoteById(int commentId);
        IEnumerable<AppUser> GetUserById(string userId);
        Comment GetMaxId();
        IEnumerable<AppUser> GetUserById();
        int Count(int postId);
        int CountProduct(int productId);
    }
    public class CommentService : ICommentService
    {
        private ICommentRepository _commentRepository;
        private IUnitOfWork _unitOfWork;
        public CommentService(ICommentRepository commentRepository, IUnitOfWork unitOfWork)
        {
            this._commentRepository = commentRepository;
            this._unitOfWork = unitOfWork;
        }
        public Comment Add(Comment commnet)
        {
            return _commentRepository.Add(commnet);
        }

        public int Count(int postId)
        {
            var query = _commentRepository.GetAll().Where(x => x.PostId == postId);
            int total = query.Count();
            return total;
        }

        public int CountProduct(int productId)
        {
            var query = _commentRepository.GetAll().Where(x => x.ProductId == productId);
            int total = query.Count();
            return total;
        }

        public Comment Delete(int id)
        {
            return _commentRepository.Delete(id);
        }

        public Comment GetById(int id)
        {
            return _commentRepository.GetSingleById(id);
        }

        public IEnumerable<Comment> GetListCommentByPostId(int? postId)
        {
            return _commentRepository.GetMulti(x=>x.PostId==postId).OrderByDescending(x=>x.CreateDate);
        }

        public IEnumerable<Comment> GetListCommentByProductId(int? productId)
        {
            return _commentRepository.GetMulti(x => x.ProductId == productId).OrderByDescending(x => x.CreateDate);
        }

        public IEnumerable<CommentVote> GetListCommentVoteById(int commentId)
        {
            return _commentRepository.getListVote(commentId);
        }

        public Comment GetMaxId()
        {
           int max = _commentRepository.GetAll().Max(x => x.ID);
           var comment = _commentRepository.GetSingleById(max);
           return comment;
        }

        public IEnumerable<AppUser> GetUserById()
        {
            int max = _commentRepository.GetAll().Max(x => x.ID);
            var comment = _commentRepository.GetSingleById(max);
            IEnumerable<AppUser> user = _commentRepository.GetUserById(comment.UserId);
            return user;
        }

        public IEnumerable<AppUser> GetUserById(string userId)
        {
            return _commentRepository.GetUserById(userId);
        }

        public IEnumerable<AppUser> GetUserByPostId(int postId)
        {
            return _commentRepository.GetListUserByPostId(postId);
        }

        public IEnumerable<AppUser> GetUserByProductId(int productId)
        {
            return _commentRepository.GetListUserByProductId(productId);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(Comment commnet)
        {
            _commentRepository.Update(commnet);
        }     
    }
}
