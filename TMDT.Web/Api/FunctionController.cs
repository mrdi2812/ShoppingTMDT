using AutoMapper;
using Microsoft.AspNet.Identity;
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
    [Authorize]
    [RoutePrefix("api/function")]
    public class FunctionController : ApiControllerBase
    {
        private IFunctionService _functionService;
        public FunctionController(IErrorService errorService, IFunctionService functionService) : base(errorService)
        {
            _functionService = functionService;
        }
        [Route("getlisthierarchy")]
        [HttpGet]
        public HttpResponseMessage GetAllHierachy(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {       
                IEnumerable<Function> model;
                if (User.IsInRole("Admin"))
                {
                    model = _functionService.GetAll(string.Empty);
                }
                else
                {
                    model = _functionService.GetAllWithPermission(User.Identity.GetUserId());
                }

                IEnumerable<FunctionViewModel> modelVm = Mapper.Map<IEnumerable<Function>, IEnumerable<FunctionViewModel>>(model);
                var parents = modelVm.Where(x => x.Parent == null);
                foreach (var parent in parents)
                {
                    parent.ChildFunctions = modelVm.Where(x => x.ParentId == parent.ID).ToList();                 
                }    
                return request.CreateResponse(HttpStatusCode.OK, parents);

            });
        }
        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request, string filter = "")
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _functionService.GetAll(filter);
                IEnumerable<FunctionViewModel> modelVm = Mapper.Map<IEnumerable<Function>, IEnumerable<FunctionViewModel>>(model);
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }
        [Route("detail/{id}")]
        [HttpGet]
        public HttpResponseMessage Details(HttpRequestMessage request, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị.");
            }
            else
            {
                var model = _functionService.GetById(id);
                if (model == null)
                {
                    return request.CreateErrorResponse(HttpStatusCode.NoContent, "No data");
                }
                var modelVm = Mapper.Map<Function, FunctionViewModel>(model);
                return request.CreateResponse(HttpStatusCode.OK, modelVm);
            }
        }
        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Create(HttpRequestMessage request, FunctionViewModel functionViewModel)
        {
            if (ModelState.IsValid)
            {
                var model = new Function();
                try
                {
                    bool check = _functionService.CheckExistedId(functionViewModel.ID);
                    if (check == true)
                    {
                        return request.CreateErrorResponse(HttpStatusCode.BadRequest, "Dữ liệu đã tồn tại");
                    }
                    if (functionViewModel.ParentId =="") { functionViewModel.ParentId = null; }
                    model.UpdateFunction(functionViewModel);
                    _functionService.Create(model);
                    _functionService.Save();
                    return request.CreateResponse(HttpStatusCode.OK, functionViewModel);
                }
                catch (Exception ex)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                }
            }
            else
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [HttpPut]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, FunctionViewModel functionViewModel)
        {
            if (ModelState.IsValid)
            {
                var model = _functionService.GetById(functionViewModel.ID);
                if (model == null)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, "ID không tồn tại");
                }
                try
                {
                    if (functionViewModel.ParentId == "") { functionViewModel.ParentId = null; }
                    model.UpdateFunction(functionViewModel);
                    _functionService.Update(model);
                    _functionService.Save();
                    return request.CreateResponse(HttpStatusCode.OK, functionViewModel);
                }
                catch (Exception ex)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                }
            }
            else
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }
        [HttpDelete]
        [Route("delete")]
        public HttpResponseMessage Delete(HttpRequestMessage request, string id)
        {
            _functionService.Delete(id);
            _functionService.Save();

            return request.CreateResponse(HttpStatusCode.OK, id);
        }
    }
}
