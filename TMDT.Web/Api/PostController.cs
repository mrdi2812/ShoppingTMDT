using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using TMDT.Model.Models;
using TMDT.Service;
using TMDT.Web.Infrastructure.Core;
using TMDT.Web.Infrastructure.Extensions;
using TMDT.Web.Models;
using TMDT.Web.Models.Common;

namespace TMDT.Web.Api
{
    [RoutePrefix("api/post")]
    [Authorize]
    public class PostController : ApiControllerBase
    {
        private IPostService _postService;

        public PostController(IErrorService errorService, IPostService postService) : base(errorService)
        {
            _postService = postService;
        }

        [Route("getallparents")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _postService.GetAll();
                var modelVm = Mapper.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(model);
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }

        [Route("getallpaging")]
        [HttpGet]
        public HttpResponseMessage GetAllPaging(HttpRequestMessage request, int page, int pageSize)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                int totalRow = 0;
                var model = _postService.GetAll();
                model = model.OrderByDescending(x => x.CreatedDate).Skip(pageSize * (page - 1)).Take(pageSize);
                totalRow = model.Count();
                var modelVm = Mapper.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(model);
                PaginationSet<PostViewModel> pagiSet = new PaginationSet<PostViewModel>()
                {
                    PageIndex = page,
                    PageSize = pageSize,
                    TotalRows = totalRow,
                    Items = modelVm
                };
                response = request.CreateResponse(HttpStatusCode.OK, pagiSet);
                return response;
            });
        }

        [Route("getallcategorypaging")]
        [HttpGet]
        public HttpResponseMessage GetAllByCategoryPaging(HttpRequestMessage request, int categoryId, int page, int pageSize)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                int totalRow = 0;
                var model = _postService.GetAllByCategoryPaging(categoryId, page, pageSize, out totalRow);
                var modelVm = Mapper.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(model);
                PaginationSet<PostViewModel> pagiSet = new PaginationSet<PostViewModel>()
                {
                    PageIndex = page,
                    PageSize = pageSize,
                    TotalRows = totalRow,
                    Items = modelVm
                };
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }

        [Route("detail/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _postService.GetById(id);
                var modelVm = Mapper.Map<Post, PostViewModel>(model);
                var response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }

        [Route("gettags")]
        [HttpGet]
        public HttpResponseMessage GetTags(HttpRequestMessage request, string text)
        {
            Func<HttpResponseMessage> func = () =>
            {
                var model = _postService.GetListPostTag(text);
                var responseData = Mapper.Map<IEnumerable<Tag>, IEnumerable<TagViewModel>>(model);
                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            };
            return CreateHttpResponse(request, func);
        }

        [Route("getallbytag")]
        [HttpGet]
        public HttpResponseMessage GetAllByTagPaging(HttpRequestMessage request, string text, int page, int pageSize)
        {
            return CreateHttpResponse(request, () =>
            {
                int totalRow = 0;
                var model = _postService.GetAllByTagPaging(text, page, pageSize, out totalRow);
                var modelVm = Mapper.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(model);
                PaginationSet<PostViewModel> pageSet = new PaginationSet<PostViewModel>()
                {
                    PageIndex = page,
                    PageSize = pageSize,
                    TotalRows = totalRow,
                    Items = modelVm
                };
                var response = request.CreateResponse(HttpStatusCode.OK, pageSet);
                return response;
            });
        }

        [Route("add")]
        [HttpPost]
        public HttpResponseMessage Create(HttpRequestMessage request, PostViewModel postVm)
        {
            return CreateHttpResponse(request, () =>
            {
                if (ModelState.IsValid)
                {
                    var post = new Post();
                    post.UpdatePost(postVm);
                    post.CreatedDate = DateTime.Now;
                    _postService.Add(post);
                    _postService.Save();
                    var result = Mapper.Map<Post, PostViewModel>(post);
                    var response = request.CreateResponse(HttpStatusCode.Created, result);
                    return response;
                }
                else
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            });
        }

        [Route("update")]
        [HttpPut]
        public HttpResponseMessage Update(HttpRequestMessage request, PostViewModel postVm)
        {
            return CreateHttpResponse(request, () =>
            {
                if (ModelState.IsValid)
                {
                    var post = _postService.GetById(postVm.ID);
                    post.UpdatePost(postVm);
                    post.UpdatedDate = DateTime.Now;
                    _postService.Update(post);
                    _postService.Save();
                    var result = Mapper.Map<Post, PostViewModel>(post);
                    var response = request.CreateResponse(HttpStatusCode.OK, result);
                    return response;
                }
                else
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            });
        }

        [Route("delete")]
        [HttpDelete]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                _postService.Delete(id);
                _postService.Save();
                response = request.CreateResponse(HttpStatusCode.OK, "Xóa thành công");
                return response;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string checkedPosts)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var listPostCategory = new JavaScriptSerializer().Deserialize<List<int>>(checkedPosts);
                foreach (var item in listPostCategory)
                {
                    _postService.Delete(item);
                }
                _postService.Save();
                response = request.CreateResponse(HttpStatusCode.OK, listPostCategory.Count);
                return response;
            });
        }
    }
}