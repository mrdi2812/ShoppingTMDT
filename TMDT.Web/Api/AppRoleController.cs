using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TMDT.Common;
using TMDT.Model.Models;
using TMDT.Service;
using TMDT.Web.Infrastructure.Core;
using TMDT.Web.Infrastructure.Extensions;
using TMDT.Web.Models;
using TMDT.Web.Models.DataContracts;

namespace TMDT.Web.Api
{
    [RoutePrefix("api/appRole")]
    [Authorize]
    public class AppRoleController : ApiControllerBase
    {
        private IPermissionService _permissionService;
        private IFunctionService _functionService;
        private IAppRoleService _appRoleService;

        public AppRoleController(IErrorService errorService, IAppRoleService appRoleService, IPermissionService permissionService, IFunctionService functionService) : base(errorService)
        {
            _functionService = functionService;
            _permissionService = permissionService;
            _appRoleService = appRoleService;
        }

        [Route("getlistpaging")]
        [HttpGet]
        public HttpResponseMessage GetListPaging(HttpRequestMessage request, int page, int pageSize, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                int totalRow = 0;
                var query = AppRoleManager.Roles;
                if (!string.IsNullOrEmpty(filter))
                    query = query.Where(x => x.Description.Contains(filter) || x.Name.Contains(filter));
                totalRow = query.Count();
                var model = query.OrderBy(X => X.Name).Skip(pageSize * (page - 1)).Take(pageSize);
                IEnumerable<ApplicationRoleViewModel> modelVm = Mapper.Map<IEnumerable<AppRole>, IEnumerable<ApplicationRoleViewModel>>(model);
                PaginationSet<ApplicationRoleViewModel> pagedSet = new PaginationSet<ApplicationRoleViewModel>
                {
                    PageIndex = page,
                    PageSize = pageSize,
                    TotalRows = totalRow,
                    Items = modelVm
                };
                response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
                return response;
            });
        }

        [Route("getlistall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = AppRoleManager.Roles.ToList();
                IEnumerable<ApplicationRoleViewModel> modelVm = Mapper.Map<IEnumerable<AppRole>, IEnumerable<ApplicationRoleViewModel>>(model);
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }

        [Route("getdetail/{id}")]
        [HttpGet]
        public HttpResponseMessage GetDetail(HttpRequestMessage request, string id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _appRoleService.GetDetail(id);
                var modelVm = Mapper.Map<AppRole, ApplicationRoleViewModel>(model);
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }

        [Route("getAllPermission")]
        [HttpGet]
        public HttpResponseMessage GetAllPermission(HttpRequestMessage request, string functionId)
        {
            return CreateHttpResponse(request, () =>
            {
                List<PermissionViewModel> permissionVm = new List<PermissionViewModel>();
                HttpResponseMessage response = null;
                var roles = AppRoleManager.Roles.Where(x => x.Name != CommonConstants.ADMIN).ToList();
                var listpermissions = _permissionService.GetByFunctionId(functionId).ToList();
                if (listpermissions.Count == 0)
                {
                    foreach (var item in roles)
                    {
                        permissionVm.Add(new PermissionViewModel
                        {
                            RoleId = item.Id,
                            FunctionId = functionId,
                            CanCreate = false,
                            CanDelete = false,
                            CanRead = false,
                            CanUpdate = false,
                            AppRole = new ApplicationRoleViewModel
                            {
                                Description = item.Description,
                                Id = item.Id,
                                Name = item.Name
                            }
                        });
                    }
                }
                else
                {
                    foreach (var item in roles)
                    {
                        if (!listpermissions.Any(x => x.RoleId == item.Id))
                        {
                            permissionVm.Add(new PermissionViewModel()
                            {
                                RoleId = item.Id,
                                CanCreate = false,
                                CanDelete = false,
                                CanRead = false,
                                CanUpdate = false,
                                AppRole = new ApplicationRoleViewModel()
                                {
                                    Id = item.Id,
                                    Description = item.Description,
                                    Name = item.Name
                                }
                            });
                        }
                        permissionVm = Mapper.Map<List<Permission>, List<PermissionViewModel>>(listpermissions);
                    }
                }
                response = request.CreateResponse(HttpStatusCode.OK, permissionVm);

                return response;
            });
        }

        [HttpPost]
        [Route("savePermission")]
        public HttpResponseMessage SavePermission(HttpRequestMessage request, SavePermissionRequest data)
        {
            if (ModelState.IsValid)
            {
                _permissionService.DeleteAll(data.FunctionId);
                Permission permission = null;
                foreach (var item in data.Permissions)
                {
                    permission = new Permission();
                    permission.UpdatePermission(item);
                    permission.FunctionId = data.FunctionId;
                    _permissionService.Add(permission);
                }
                var functions = _functionService.GetAllWithParentID(data.FunctionId);
                if (functions.Any())
                {
                    foreach (var item in functions)
                    {
                        if (_permissionService.CheckContaint(item.ID))
                        {
                            _permissionService.DeleteAll(item.ID);
                        }
                        foreach (var p in data.Permissions)
                        {
                            var childPermission = new Permission();
                            childPermission.FunctionId = item.ID;
                            childPermission.RoleId = p.RoleId;
                            childPermission.CanRead = p.CanRead;
                            childPermission.CanCreate = p.CanCreate;
                            childPermission.CanDelete = p.CanDelete;
                            childPermission.CanUpdate = p.CanUpdate;
                            _permissionService.Add(childPermission);
                        }
                    }
                }
                try
                {
                    _permissionService.SaveChange();
                    return request.CreateResponse(HttpStatusCode.OK, "Lưu quyền thành cống");
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

        [Route("add")]
        [HttpPost]
        public HttpResponseMessage Add(HttpRequestMessage request, ApplicationRoleViewModel appRoleVm)
        {
            return CreateHttpResponse(request, () =>
            {
                if (ModelState.IsValid)
                {
                    HttpResponseMessage response = null;
                    var newRole = new AppRole();
                    newRole.UpdateRole(appRoleVm);
                    newRole.Id = Guid.NewGuid().ToString();//sinh ngẫu nhiên id
                    _appRoleService.Add(newRole);
                    _appRoleService.Save();
                    var modelVm = Mapper.Map<AppRole, ApplicationRoleViewModel>(newRole);
                    response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                    return response;
                }
                else
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            });
        }

        [Route("update")]
        [HttpPut]
        public HttpResponseMessage Update(HttpRequestMessage request, ApplicationRoleViewModel appRoleVm)
        {
            return CreateHttpResponse(request, () =>
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var newRole = _appRoleService.GetDetail(appRoleVm.Id);
                        newRole.UpdateRole(appRoleVm);
                        _appRoleService.Update(newRole);
                        _appRoleService.Save();
                        var modelVm = Mapper.Map<AppRole, ApplicationRoleViewModel>(newRole);
                        return request.CreateResponse(HttpStatusCode.OK, modelVm);
                    }
                    catch (Exception dex)
                    {
                        return request.CreateErrorResponse(HttpStatusCode.BadRequest, dex.Message);
                    }
                }
                else
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            });
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(HttpRequestMessage request, string id)
        {
            var role = await AppRoleManager.FindByIdAsync(id);
            var listPermission = _permissionService.GetByRoleId(id);
            if (listPermission.Count > 0)
            {              
                    _permissionService.Delete(id);
            }
            var result = await AppRoleManager.DeleteAsync(role);  
            if (result.Succeeded)
            {
                return request.CreateResponse(HttpStatusCode.OK, "Xóa thành công" + nameof(id));
            }
            else
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join(",", result.Errors));
        }
    }
}