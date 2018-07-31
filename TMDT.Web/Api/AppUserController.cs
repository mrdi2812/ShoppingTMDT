using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TMDT.Common.Exceptions;
using TMDT.Service;
using TMDT.Model.Models;
using TMDT.Web.Infrastructure.Core;
using TMDT.Web.Providers;
using TMDT.Web.Models;
using TMDT.Web.Infrastructure.Extensions;

namespace TMDT.Web.Api
{
    [RoutePrefix("api/appUser")]
    [Authorize]
    public class AppUserController : ApiControllerBase
    {
        public AppUserController(IErrorService errorService) : base(errorService)
        {

        }
        [Route("getlistpaging")]
        [HttpGet]
        [Permission(Action = "Read", Function = "USER")]
        public HttpResponseMessage GetListPaging(HttpRequestMessage request, int page, int pageSize, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                int totalRow = 0;
                var data = AppUserManager.Users;
                if (!string.IsNullOrEmpty(filter))
                    data = data.Where(x => x.UserName.Contains(filter) || x.FullName.Contains(filter));
                totalRow = data.Count();
                var model = data.OrderBy(x => x.FullName).Skip(pageSize * (page - 1)).Take(pageSize);
                IEnumerable<ApplicationUserViewModel> modelVm = Mapper.Map<IEnumerable<AppUser>, IEnumerable<ApplicationUserViewModel>>(data);
                PaginationSet<ApplicationUserViewModel> pagedSet = new PaginationSet<ApplicationUserViewModel>
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
        [Route("detail/{id}")]
        [HttpGet]
        [Permission(Action = "Read", Function = "USER")]
        public async Task<HttpResponseMessage> Details(HttpRequestMessage request, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị.");
            }
            var user = await AppUserManager.FindByIdAsync(id);
            if (user == null)
            {
                return request.CreateErrorResponse(HttpStatusCode.NoContent, "Không có dữ liệu");
            }
            else
            {
                var role = await AppUserManager.GetRolesAsync(user.Id);
                var appUserVm = Mapper.Map<AppUser, ApplicationUserViewModel>(user);
                appUserVm.BirthDay = DateTime.Parse(appUserVm.BirthDay).ToShortDateString();
                appUserVm.Roles = role;
                return request.CreateResponse(HttpStatusCode.OK, appUserVm);
            }
        }
        [HttpPost]
        [Route("add")]
        [Permission(Action = "Create", Function = "USER")]
        public async Task<HttpResponseMessage> Create(HttpRequestMessage request, ApplicationUserViewModel applicationUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var newAppUser = new AppUser();
                newAppUser.UpdateUser(applicationUserViewModel);
                try
                {
                    newAppUser.Id = Guid.NewGuid().ToString();//sinh ngẫu nhiên id 
                    var result = await AppUserManager.CreateAsync(newAppUser, applicationUserViewModel.Password);
                    if (result.Succeeded)
                    {
                        var roles = applicationUserViewModel.Roles.ToArray();            
                        await AppUserManager.AddToRolesAsync(newAppUser.Id, roles);                       
                        return request.CreateResponse(HttpStatusCode.OK, applicationUserViewModel);
                    }
                    else
                        return request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join(",", result.Errors));
                }
                catch (NameDuplicatedException dex)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, dex.Message);
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
        [Permission(Action = "Update", Function = "USER")]
        public async Task<HttpResponseMessage> Update(HttpRequestMessage request, ApplicationUserViewModel applicationUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var appUser = await AppUserManager.FindByIdAsync(applicationUserViewModel.Id);
                try
                {
                    appUser.UpdateUser(applicationUserViewModel);
                    var result = await AppUserManager.UpdateAsync(appUser);
                    if (result.Succeeded)
                    {
                        var userRoles = await AppUserManager.GetRolesAsync(appUser.Id);
                        var selectedRole = applicationUserViewModel.Roles.ToArray();

                        selectedRole = selectedRole ?? new string[] { };//kiểm tra rỗng
                        //await AppUserManager.RemoveFromRolesAsync(appUser.Id,userRoles.ToArray());
                        await AppUserManager.AddToRolesAsync(appUser.Id, selectedRole.Except(userRoles).ToArray());//add role không thuộc trong userRoles vào qua Except
                        return request.CreateResponse(HttpStatusCode.OK, applicationUserViewModel);
                    }
                    else
                        return request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join(",", result.Errors));
                }
                catch (NameDuplicatedException dex)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, dex.Message);
                }
            }
            else
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [HttpDelete]
        [Route("delete")]
        [Permission(Action = "Delete", Function = "USER")]
        public async Task<HttpResponseMessage> Delete(HttpRequestMessage request, string id)
        {
            var appUser = await AppUserManager.FindByIdAsync(id);
            var result = await AppUserManager.DeleteAsync(appUser);
            if (result.Succeeded)
                return request.CreateResponse(HttpStatusCode.OK, id);
            else
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join(",", result.Errors));
        }
    }
}
