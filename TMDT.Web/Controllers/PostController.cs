using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Common;
using TMDT.Model.Models;
using TMDT.Service;
using TMDT.Web.App_Start;
using TMDT.Web.Infrastructure.Core;
using TMDT.Web.Models;
using TMDT.Web.Models.Common;

namespace TMDT.Web.Controllers
{
    public class PostController : Controller
    {
        // GET: Post
        IPostService _postService;
        IPostCategoryService _postCategoryService;
        ICommentService _commentService;
        ICommentVoteService _commentVoteService;
        private ApplicationUserManager _userManager;
        public PostController(IPostService postService, IPostCategoryService postCategoryService, ICommentService commentService, ICommentVoteService commentVoteService, ApplicationUserManager userManager)
        {
            _postService = postService;
            _postCategoryService = postCategoryService;
            _commentService = commentService;
            _userManager = userManager;
            _commentVoteService = commentVoteService;
        }
        public ActionResult Category(int id, int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetByKey("PageSize"));
            int totalRow = 0;
            var postModel = _postService.GetAllByCategoryPaging(id, page, pageSize, out totalRow);
            var postViewModel = Mapper.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(postModel);
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);
            var category = _postCategoryService.GetById(id);
            ViewBag.Category = Mapper.Map<PostCategory, PostCategoryViewModel>(category);
            var paginationSet = new PaginationSet<PostViewModel>()
            {
                Items = postViewModel,
                MaxPage = int.Parse(ConfigHelper.GetByKey("MaxPage")),
                PageIndex = page,
                TotalRows = totalRow,
                TotalPages = totalPage
            };
            return View(paginationSet);
        }
        public ActionResult Detail(int postId)
        {
            int top = int.Parse(ConfigHelper.GetByKey("Top"));
            var postModel = _postService.GetById(postId);       
            if (postModel != null)
            {
                postModel.ViewCount += 1;
                _postService.Update(postModel);
                _postService.Save();
            }
            var viewModel = Mapper.Map<Post, PostViewModel>(postModel);
            var relatedPost = _postService.GetReatePosts(postId, top);
            ViewBag.RelatedProducts = Mapper.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(relatedPost);
            ViewBag.PostId = postId;
            int total = _commentService.Count(postId);
            if (Request.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var userNameId = User.Identity.GetUserName();
                ViewBag.UserId = userId;
                ViewBag.UserNameId = userNameId;
                ViewBag.TotalCount = total;
            }
            ViewBag.Tags = Mapper.Map<IEnumerable<Tag>, IEnumerable<TagViewModel>>(_postService.GetListTagByPostId(postId));
            return View(viewModel);
        }
    }
}