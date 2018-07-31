using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TMDT.Service;
using TMDT.Web.Infrastructure.Core;
using TMDT.Web.Models.Common;
using TMDT.Model.Models;
using TMDT.Web.Infrastructure.Extensions;

namespace TMDT.Web.Api
{
    [RoutePrefix("api/menu")]
    public class MenuController : ApiControllerBase
    {
        private IMenuService _menuService;
        public MenuController(IErrorService errorService, IMenuService menuService) : base(errorService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        [Route("getall")]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                //int totalRows = 0;
                HttpResponseMessage response = null;
                var model = _menuService.GetAll();
                //totalRows = model.Count();
                //model = model.OrderBy(x => x.Name).Skip(pageSize * (page - 1)).Take(pageSize);
                var modelVm = Mapper.Map<IEnumerable<Menu>, IEnumerable<MenuViewModel>>(model);
                //PaginationSet<MenuViewModel> pagiSet = new PaginationSet<MenuViewModel>()
                //{
                //    Items = modelVm,
                //    PageIndex = page,
                //    PageSize = pageSize,
                //    TotalRows = totalRows
                //};
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }
        [Route("getlisthierarchy")]
        [HttpGet]
        public HttpResponseMessage GetAllHierachy(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _menuService.GetAll();
                var modelVm = Mapper.Map<IEnumerable<Menu>, IEnumerable<MenuViewModel>>(model);
                var parents = modelVm.Where(x => x.Parent == null);
                foreach (var parent in parents)
                {
                    parent.ChildMenus = modelVm.Where(x => x.ParentId == parent.ID).ToList();
                }
                return request.CreateResponse(HttpStatusCode.OK, parents);
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
                var model = _menuService.GetById(id);
                if (model == null)
                {
                    return request.CreateErrorResponse(HttpStatusCode.NoContent, "No data");
                }
                var modelVm = Mapper.Map<Menu, MenuViewModel>(model);
                return request.CreateResponse(HttpStatusCode.OK, modelVm);
            }
        }
        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Add(HttpRequestMessage request,MenuViewModel modelVm)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = null;
                try
                {
                    var model = new Menu();
                    model.UpdateMenu(modelVm);
                    var result = _menuService.Add(model);
                    _menuService.Save();
                    var menuVm = Mapper.Map<Menu, MenuViewModel>(result);
                    response = request.CreateResponse(HttpStatusCode.OK, menuVm);
                    return response;
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
        public HttpResponseMessage Update(HttpRequestMessage request, MenuViewModel menuVm)
        {
            if (ModelState.IsValid)
            {
                var model = _menuService.GetById(menuVm.ID);
                if (model == null)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, "ID không tồn tại");
                }
                try
                {
                    if (menuVm.ParentId == "") { menuVm.ParentId = null; }
                    model.UpdateMenu(menuVm);
                    _menuService.Update(model);
                    _menuService.Save();
                    return request.CreateResponse(HttpStatusCode.OK, menuVm);
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
            _menuService.Delete(id);
            _menuService.Save();
            return request.CreateResponse(HttpStatusCode.OK, id);
        }
    }
}
