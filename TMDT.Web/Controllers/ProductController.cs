using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Common;
using TMDT.Service;
using TMDT.Web.Models;
using TMDT.Model.Models;
using TMDT.Web.Infrastructure.Core;
using TMDT.Web.Models.Common;
using System.Web.Script.Serialization;
using TMDT.Common.ViewModels;
using Microsoft.AspNet.Identity;
using TMDT.Web.Infrastructure.Extensions;

namespace TMDT.Web.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        IProductService _productService;
        IProductQuantityService _productQuantityService;
        IProductCategoryService _productCategoryService;
        IProductImageService _productImageService;
        ICommentService _commentService;
        ICommentVoteService _commentVoteService;
        public ProductController(IProductService productService, IProductQuantityService productQuantityService,
            IProductCategoryService productCategoryService, IProductImageService productImageService, ICommentService commentService
            , ICommentVoteService commentVoteService)
        {
            _productService = productService;
            _productQuantityService = productQuantityService;
            _productCategoryService = productCategoryService;
            _productImageService = productImageService;
            _commentService = commentService;
            _commentVoteService = commentVoteService;
        }
        public ActionResult Index(int id,int page = 1, string sort = "")
        {
            int pageSize = int.Parse(ConfigHelper.GetByKey("PageSize"));
            int totalRow = 0;
            var productModel = _productService.GetListProductByCategoryIdPaging(id, page, pageSize, sort, out totalRow);
            var productViewModel = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(productModel);
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);
            var category = _productCategoryService.GetById(id);
            ViewBag.Category = Mapper.Map<ProductCategory, ProductCategoryViewModel>(category);
            var paginationSet = new PaginationSet<ProductViewModel>()
            {
                Items = productViewModel,
                MaxPage = int.Parse(ConfigHelper.GetByKey("MaxPage")),
                PageIndex = page,
                TotalRows = totalRow,
                TotalPages = totalPage
            };
            return View(paginationSet);
        }
        public ActionResult _MenuLeft()
        {
            var shopVm = new ShopViewModel();
            var color = _productQuantityService.GetListColor(null);
            var size = _productQuantityService.GetListSize(null);
            var category = _productCategoryService.GetAll();
            var colorVm = Mapper.Map<IEnumerable<Color>, IEnumerable<ColorViewModel>>(color);
            var sizeVm = Mapper.Map<IEnumerable<Size>, IEnumerable<SizeViewModel>>(size);
            var categoryVm = Mapper.Map<IEnumerable<ProductCategory>, IEnumerable<ProductCategoryViewModel>>(category);
            shopVm.Colors = colorVm;
            shopVm.Sizes = sizeVm;
            shopVm.ProductCategory = categoryVm;
            return PartialView(shopVm);
        }
        public ActionResult Detail(int id)
        {
            int top = int.Parse(ConfigHelper.GetByKey("Top"));
            var productModel = _productService.GetById(id);
            _productService.IncreaseView(productModel.ID);
            _productService.Update(productModel);
            _productService.Save();
            var viewModel = Mapper.Map<Product, ProductViewModel>(productModel);
            var image = _productImageService.GetAll(id);
            var imageVm = Mapper.Map<IEnumerable<ProductImage>,IEnumerable<ProductImageViewModel>>(image);
            var listSizeColor = _productQuantityService.GetListByProduct(id);
            var relatedProduct = _productService.GetReatedProducts(id, top);
            ViewBag.RelatedProducts = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(relatedProduct);          
            ViewBag.MoreImages = imageVm;
            var tag = _productService.GetListTagByProductId(id);
            ViewBag.Tags = Mapper.Map<IEnumerable<Tag>, IEnumerable<TagViewModel>>(tag);
            ViewBag.Size = listSizeColor;
            int total = _commentService.CountProduct(id);
            if (Request.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var userNameId = User.Identity.GetUserName();
                ViewBag.UserId = userId;
                ViewBag.UserNameId = userNameId;
                ViewBag.TotalCount = total;
            }
            return View(viewModel);
        }
        public ActionResult Search(string keyword, int page = 1, string sort = "")
        {
            int pageSize = int.Parse(ConfigHelper.GetByKey("PageSize"));
            int totalRow = 0;
            var productModel = _productService.Search(keyword, page, pageSize, sort, out totalRow);
            var productViewModel = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(productModel);
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);
            ViewBag.Keyword = keyword;
            var paginationSet = new PaginationSet<ProductViewModel>()
            {
                Items = productViewModel,
                MaxPage = int.Parse(ConfigHelper.GetByKey("MaxPage")),
                PageIndex = page,
                TotalRows = totalRow,
                TotalPages = totalPage
            };
            return View(paginationSet);
        }
        public ActionResult ListByTag(string tagId, int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetByKey("PageSize"));
            int totalRow = 0;
            var productModel = _productService.GetListProductByTag(tagId, page, pageSize, out totalRow);
            var productViewModel = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(productModel);
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);
            var tag = _productService.GetTag(tagId);
            ViewBag.Name = tag.Name;
            ViewBag.Tag = Mapper.Map<Tag, TagViewModel>(_productService.GetTag(tagId));
            var paginationSet = new PaginationSet<ProductViewModel>()
            {
                Items = productViewModel,
                MaxPage = int.Parse(ConfigHelper.GetByKey("MaxPage")),
                PageIndex = page,
                TotalRows = totalRow,
                TotalPages = totalPage
            };
            return View(paginationSet);
        }
        public JsonResult GetAllUser(int id)
        {
            var users = _commentService.GetUserByProductId(id);
            var userVm = Mapper.Map<IEnumerable<AppUser>, IEnumerable<ApplicationUserViewModel>>(users);
            if (userVm != null)
            {
                return Json(new
                {
                    data = userVm,
                    status = true
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                status = false
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAll(int id)
        {
            var users = _commentService.GetUserByProductId(id);
            var userVm = Mapper.Map<IEnumerable<AppUser>, IEnumerable<ApplicationUserViewModel>>(users);
            var comments = _commentService.GetListCommentByProductId(id);
            foreach (var item in comments)
            {
                var vote = _commentService.GetListCommentVoteById(item.ID);
                var voteVm = Mapper.Map<IEnumerable<CommentVote>, IEnumerable<CommentVoteViewModel>>(vote);
            }
            var model = Mapper.Map<IEnumerable<Comment>, IEnumerable<CommentViewModel>>(comments);
            if (model != null)
            {
                return Json(new
                {
                    data = model,
                    status = true
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                status = false
            }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult Insert(CommentViewModel commentObj)
        {
            commentObj.CreateBy = User.Identity.GetUserName();
            commentObj.CreateDate = DateTime.Now;
            commentObj.ModifiedDate = DateTime.Now;
            commentObj.ReplyCount = 0;
            commentObj.UserId = User.Identity.GetUserId();
            commentObj.Status = true;
            if (ModelState.IsValid)
            {
                var comment = new Comment();
                comment.UpdateComment(commentObj);
                _commentService.Add(comment);
                _commentService.Save();
                var model = _commentService.GetMaxId();
                var user = _commentService.GetUserById(model.UserId);
                var userVm = Mapper.Map<IEnumerable<AppUser>, IEnumerable<ApplicationUserViewModel>>(user);
                var commentVm = Mapper.Map<Comment, CommentViewModel>(model);
                commentObj.AppUser = commentVm.AppUser;
                commentObj.ID = commentVm.ID;
            }

            return Json(new
            {
                data = commentObj,
                status = true
            }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult Update(CommentViewModel commentObj)
        {
            var comment = _commentService.GetById(commentObj.ID);
            comment.Content = commentObj.Content;
            comment.ModifiedDate = DateTime.Now;
            _commentService.Update(comment);
            _commentService.Save();
            var user = _commentService.GetUserById(comment.UserId);
            var userVm = Mapper.Map<IEnumerable<AppUser>, IEnumerable<ApplicationUserViewModel>>(user);
            var comments = _commentService.GetListCommentByProductId(comment.ProductId);
            foreach (var item in comments)
            {
                var vote = _commentService.GetListCommentVoteById(item.ID);
                var voteVm = Mapper.Map<IEnumerable<CommentVote>, IEnumerable<CommentVoteViewModel>>(vote);
            }
            var commentVm = Mapper.Map<Comment, CommentViewModel>(comment);
            commentObj.CreateBy = commentVm.CreateBy;
            commentObj.CreateDate = commentVm.CreateDate;
            commentObj.ModifiedDate = DateTime.Now;
            commentObj.ReplyCount = commentVm.ReplyCount;
            commentObj.UserId = commentVm.UserId;
            commentObj.Status = commentVm.Status;
            commentObj.ParentId = commentVm.ParentId;
            commentObj.PostId = null;
            commentObj.ProductId = commentVm.ProductId;
            commentObj.AppUser = commentVm.AppUser;
            commentObj.CommentVotes = commentVm.CommentVotes;
            return Json(new
            {
                data = commentObj
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Vote(int commentId, string currentUserId)
        {
            int checkvote = _commentVoteService.CheckVote(currentUserId, commentId);
            var vote = new CommentVote();
            vote.CommentId = commentId;
            vote.UserId = User.Identity.GetUserId();
            var listVote = _commentVoteService.GetAll();
            if (checkvote == 0)
            {
                _commentVoteService.Add(vote);
                _commentVoteService.Save();

                return Json(new
                {
                    status = true
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                _commentVoteService.Delete(currentUserId, commentId);
                _commentVoteService.Save();
            }
            return Json(new
            {
                status = false
            }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public JsonResult CheckVote(string currentUserId, int commentId)
        {
            int total = _commentVoteService.CheckVote(currentUserId, commentId);
            if (total > 0)
            {
                return Json(new
                {
                    data = total,
                    status = true
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                data = total,
                status = false
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Detele(int id)
        {
            if (ModelState.IsValid)
            {
                _commentService.Delete(id);
                _commentService.Save();
            }
            return Json("");
        }
    }
}