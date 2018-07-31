using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TMDT.Model.Models;
using TMDT.Service;
using TMDT.Web.Infrastructure.Core;
using TMDT.Web.Models.Common;
using TMDT.Web.SignalR;

namespace TMDT.Web.Api
{
    [RoutePrefix("api/Announcement")]
    public class AnnouncementController : ApiControllerBase
    {
        private IAnnouncementService _announcementService;
        public AnnouncementController(IErrorService errorService, IAnnouncementService announcementService) : base(errorService)
        {
            _announcementService = announcementService;
        }
        [Route("getTopMyAnnouncement")]
        [HttpGet]
        public HttpResponseMessage GetTopMyAnnouncement(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                int totalRow = 0;
                List<Announcement> model = _announcementService.ListAllUnread(User.Identity.GetUserId(), 1, 10, out totalRow);
                IEnumerable<AnnouncementViewModel> modelVm = Mapper.Map<List<Announcement>, List<AnnouncementViewModel>>(model);

                PaginationSet<AnnouncementViewModel> pagedSet = new PaginationSet<AnnouncementViewModel>()
                {
                    PageIndex = 1,
                    TotalRows = totalRow,
                    PageSize = 10,
                    Items = modelVm
                };
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);

                return response;
            });
        }
        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request, int pageIndex, int pageSize)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                int totalRow = 0;
                var model = _announcementService.GetListAll(pageIndex, pageSize, out totalRow);
                IEnumerable<AnnouncementViewModel> modelVm = Mapper.Map<List<Announcement>, List<AnnouncementViewModel>>(model);
                PaginationSet<AnnouncementViewModel> pagedSet = new PaginationSet<AnnouncementViewModel>()
                {
                    PageIndex = pageIndex,
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
        public HttpResponseMessage Details(HttpRequestMessage request, int id)
        {
            if (id == 0)
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị.");
            }
            var announcement = _announcementService.GetDetail(id);
            if (announcement == null)
            {
                return request.CreateErrorResponse(HttpStatusCode.NoContent, "No data");
            }
            var modelVm = Mapper.Map<Announcement, AnnouncementViewModel>(announcement);

            return request.CreateResponse(HttpStatusCode.OK, modelVm);
        }
        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Create(HttpRequestMessage request, AnnouncementViewModel announcementVm)
        {
            if (ModelState.IsValid)
            {
                var newAnnoun = new Announcement();
                try
                {
                    newAnnoun.Content = announcementVm.Content;
                    newAnnoun.Status = announcementVm.Status;
                    newAnnoun.Title = announcementVm.Title;
                    newAnnoun.CreatedDate = DateTime.Now;
                    newAnnoun.UserId = User.Identity.GetUserId();
                    var listUser = AppUserManager.Users;
                    //foreach (var user in announcementVm.AnnouncementUsers)
                    //{
                    //    newAnnoun.AnnouncementUsers.Add(new AnnouncementUser()
                    //    {
                    //        UserId = user.UserId,
                    //        HasRead = false
                    //    });
                    //} 
                    foreach (var user in listUser)
                    {
                        newAnnoun.AnnouncementUsers.Add(new AnnouncementUser()
                        {
                            UserId = user.Id,
                            HasRead = false
                        });
                    }
                    _announcementService.Create(newAnnoun);                  
                    _announcementService.Save();
                    var listuser = _announcementService.ListByNotificationId(newAnnoun.ID);
                    var announ = _announcementService.GetDetail(newAnnoun.ID);
                    var appuser = AppUserManager.FindById(newAnnoun.UserId);
                    announ.AppUser = appuser;
                    var announVm = Mapper.Map<Announcement, AnnouncementViewModel>(announ);
                    announVm.AnnouncementUsers = null;
                    //push notification, push user vào signaIR
                    TMDTHub.PushToAllUsers(announVm, null);

                    return request.CreateResponse(HttpStatusCode.OK, announcementVm);

                }
                catch (Exception dex)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, dex.Message);
                }
            }
            else
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
        [HttpGet]
        [Route("markAsRead")]
        public HttpResponseMessage MarkAsRead(HttpRequestMessage request, int announId)
        {
            try
            {
                _announcementService.MarkAsRead(User.Identity.GetUserId(), announId);
                _announcementService.Save();
                return request.CreateResponse(HttpStatusCode.OK, announId);


            }
            catch (Exception ex)
            {
                return request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            var list = _announcementService.ListByNotificationId(id);
            foreach(var item in list)
            {
                _announcementService.DeleteAllUser(item);
            }
            _announcementService.Delete(id);
            _announcementService.Save();
            return request.CreateResponse(HttpStatusCode.OK, id);
        }
    }
}
