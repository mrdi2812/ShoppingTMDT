using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TMDT.Model.Models;
using TMDT.Service;
using TMDT.Web.Infrastructure.Core;
using TMDT.Web.Infrastructure.Extensions;
using TMDT.Web.Models.Common;

namespace TMDT.Web.Api
{
    [RoutePrefix("api/slide")]
    public class SlideController : ApiControllerBase
    {
        ICommonService _commonService;
        public SlideController(IErrorService errorService, ICommonService commonService) : base(errorService)
        {
            _commonService = commonService;
        }
        [Route("getallpaging")]
        [HttpGet]
        public HttpResponseMessage GetAllPaging(HttpRequestMessage request, int page, int pageSize)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                int totalRow = 0;
                var model = _commonService.GetAllSlide();
                model = model.OrderByDescending(x => x.Status).Skip(pageSize * (page - 1)).Take(pageSize);
                totalRow = model.Count();
                var modelVm = Mapper.Map<IEnumerable<Slide>, IEnumerable<SlideViewModel>>(model);
                PaginationSet<SlideViewModel> pagiSet = new PaginationSet<SlideViewModel>()
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
        [Route("detail/{id}")]
        [HttpGet]
        public HttpResponseMessage Details(HttpRequestMessage request, int id)
        {          
                var model = _commonService.GetById(id);
                if (model == null)
                {
                    return request.CreateErrorResponse(HttpStatusCode.NoContent, "No data");
                }
                var modelVm = Mapper.Map<Slide, SlideViewModel>(model);
                return request.CreateResponse(HttpStatusCode.OK, modelVm);
        }
        [HttpPost]
        [Route("addslide")]
        public HttpResponseMessage AddSlide(HttpRequestMessage request, SlideViewModel modelVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = new Slide();
                model.UpdateSlide(modelVm);
                _commonService.Add(model);
                _commonService.Save();
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }
        [HttpPut]
        [Route("updateslide")]
        public HttpResponseMessage UpdateSlide(HttpRequestMessage request, SlideViewModel modelVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _commonService.GetById(modelVm.ID);
                model.UpdateSlide(modelVm);
                _commonService.UpdateSlide(model);
                _commonService.Save();
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }
        [HttpDelete]
        [Route("deleteslide")]
        public HttpResponseMessage DeleteSlide(HttpRequestMessage request, int id)
        {
            _commonService.DeleteSlide(id);
            _commonService.Save();
            return request.CreateResponse(HttpStatusCode.OK, id);
        }
    }
}
