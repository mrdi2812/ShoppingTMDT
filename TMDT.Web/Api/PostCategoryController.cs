using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TMDT.Service;
using TMDT.Model.Models;
using TMDT.Web.Infrastructure.Core;
using TMDT.Web.Providers;
using TMDT.Web.Models;
using TMDT.Web.Infrastructure.Extensions;

namespace TMDT.Web.Api
{
    [RoutePrefix("api/postcategory")]
    [Authorize]
    public class PostCategoryController : ApiControllerBase
    {
        IPostCategoryService _postCategoryService;
        public PostCategoryController(IErrorService errorService, IPostCategoryService postCategoryService) : base(errorService)
        {
            _postCategoryService = postCategoryService;
        }
        [Route("getall")]
        public HttpResponseMessage Get(HttpRequestMessage request,string filter,int pageSize,int page)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _postCategoryService.GetAll(filter);
                model = model.OrderByDescending(x => x.CreatedDate).Skip(pageSize * (page - 1)).Take(pageSize);
                var modelVm = Mapper.Map<IEnumerable<PostCategory>, IEnumerable<PostCategoryViewModel>>(model);
                PaginationSet<PostCategoryViewModel> pagiSet = new PaginationSet<PostCategoryViewModel>()
                {
                    PageIndex = page,
                    PageSize = pageSize,
                    TotalRows = modelVm.Count(),
                    Items = modelVm
                };
                return request.CreateResponse(HttpStatusCode.OK,pagiSet);
            });
        }
        [Route("getallparent")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _postCategoryService.GetAll();               
                var modelVm = Mapper.Map<IEnumerable<PostCategory>, IEnumerable<PostCategoryViewModel>>(model);               
                return request.CreateResponse(HttpStatusCode.OK, modelVm);
            });
        }
        [HttpGet]
        [Route("detail/{id:int}")]
        public HttpResponseMessage Detail(HttpRequestMessage request,int id)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _postCategoryService.GetById(id);
                var modelVm = Mapper.Map<PostCategory,PostCategoryViewModel>(model);
                return request.CreateResponse(HttpStatusCode.OK, modelVm);
            });
        }
        [Route("add")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Post(HttpRequestMessage request, PostCategoryViewModel postCategoryVm)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = null;
                var model = new PostCategory();
                model.UpdatePostCategory(postCategoryVm);
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User.Identity.Name;
                _postCategoryService.Add(model);
                _postCategoryService.Save();
                var result = Mapper.Map<PostCategory, PostCategoryViewModel>(model);
                response = request.CreateResponse(HttpStatusCode.OK, result);
                return response;
            }
            else
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
        [Route("update")]
        [HttpPut]
        [AllowAnonymous]
        public HttpResponseMessage Put(HttpRequestMessage request, PostCategoryViewModel postCategoryVm)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = null;
                var model = _postCategoryService.GetById(postCategoryVm.ID);
                model.UpdatePostCategory(postCategoryVm);
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = User.Identity.Name;
                _postCategoryService.Update(model);
                _postCategoryService.Save();
                var result = Mapper.Map<PostCategory, PostCategoryViewModel>(model);
                response = request.CreateResponse(HttpStatusCode.OK, result);
                return response;
            }
            else
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
        [Route("delete")]
        [HttpDelete]
        [AllowAnonymous]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;          
                _postCategoryService.Delete(id);
                _postCategoryService.Save();
                response = request.CreateResponse(HttpStatusCode.OK,id);
                return response;
            });
        }
    }
}
