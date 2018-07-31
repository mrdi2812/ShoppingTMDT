using System;
using System.Collections.Generic;
using System.Linq;
using TMDT.Common;
using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface IPostService
    {
        Post Add(Post post);

        void Update(Post post);

        void Delete(int id);

        IEnumerable<Post> GetAll();

        IEnumerable<Post> GetAllPaging(int page, int pageSize, out int totalRow);

        IEnumerable<Post> GetAllByCategoryPaging(int categoryId, int page, int pageSize, out int totalRow);

        Post GetById(int id);

        IEnumerable<Post> GetAllByTagPaging(string tag, int page, int pageSize, out int totalRow);

        IEnumerable<Tag> GetListPostTag(string searchText);

        IEnumerable<Tag> GetListTagByPostId(int id);
        void Save();
        IEnumerable<Post> GetReatePosts(int postId,int top);
    }

    public class PostService : IPostService
    {
        private IUnitOfWork _unitOfWork;
        private IPostRepository _postRepository;
        private ITagRepository _tagRepository;

        public PostService(IUnitOfWork unitOfWork, IPostRepository postRepository, ITagRepository tagRepository)
        {
            _unitOfWork = unitOfWork;
            _postRepository = postRepository;
            _tagRepository = tagRepository;
        }

        public Post Add(Post post)
        {
            return _postRepository.Add(post);
        }

        public void Update(Post post)
        {
            _postRepository.Update(post);
        }

        public void Delete(int id)
        {
            _postRepository.Delete(id);
        }

        public IEnumerable<Post> GetAll()
        {
            var model =  _postRepository.GetAll(new string[] { "PostCategory" });
            return model;
        }

        public IEnumerable<Post> GetAllPaging(int page, int pageSize, out int totalRow)
        {
            return _postRepository.GetMultiPaging(x => x.Status, out totalRow, page, pageSize);
        }

        public IEnumerable<Post> GetAllByCategoryPaging(int categoryId, int page, int pageSize, out int totalRow)
        {
            return _postRepository.GetMultiPaging(x => x.Status && x.CategoryID == categoryId, out totalRow, page, pageSize, new string[] { "PostCategory" });
        }

        public Post GetById(int id)
        {
            return _postRepository.GetSingleById(id);
        }

        public IEnumerable<Post> GetAllByTagPaging(string tag, int page, int pageSize, out int totalRow)
        {
            return _postRepository.GetAllByTag(tag, page, pageSize, out totalRow);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Tag> GetListPostTag(string searchText)
        {
            return _tagRepository.GetMulti(x => x.Type == CommonConstants.PostTag && x.Name.Contains(searchText));
        }

        public IEnumerable<Post> GetReatePosts(int postId, int top)
        {
            var product = _postRepository.GetSingleById(postId);
            return _postRepository.GetMulti(x => x.Status && x.ID != postId && x.CategoryID == product.CategoryID).OrderByDescending(x => x.CreatedDate).Take(top);
        }

        public IEnumerable<Tag> GetListTagByPostId(int id)
        {
            return _tagRepository.GetListTagByPostID(id);
        }
    }
}