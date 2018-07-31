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
using TMDT.Web.Models.Common;

namespace TMDT.Web.Api
{
    [RoutePrefix("api/error")]
    public class ErrorController : ApiControllerBase
    {
        private IErrorService _errorService;
        public ErrorController(IErrorService errorService) : base(errorService)
        {
            _errorService = errorService;
        }
        [HttpGet]
        [Route("getall")]
        public HttpResponseMessage GetAll(HttpRequestMessage request,int page, int pageSize)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _errorService.GetAll();
                model = model.OrderByDescending(x => x.CreateDate).Skip(pageSize * (page - 1)).Take(pageSize);
                var modelVm = Mapper.Map<IEnumerable<Error>, IEnumerable<ErrorViewModel>>(model);
                PaginationSet<ErrorViewModel> pagedSet = new PaginationSet<ErrorViewModel>
                {
                    PageIndex = page,
                    PageSize = pageSize,
                    TotalRows = modelVm.Count(),
                    Items = modelVm
                };
                response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
                return response;
            });
        }
    }
}
