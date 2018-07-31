using AutoMapper;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TMDT.Model.Models;
using TMDT.Service;
using TMDT.Web.Infrastructure.Core;
using TMDT.Web.Infrastructure.Extensions;
using TMDT.Web.Models;

namespace TMDT.Web.Api
{
    [RoutePrefix("api/productImage")]
    public class ProductImageController : ApiControllerBase
    {
        private IProductImageService _productImageService;

        public ProductImageController(IErrorService errorService, IProductImageService productImageService) : base(errorService)
        {
            _productImageService = productImageService;
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request, int productId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _productImageService.GetAll(productId);
                var modelVm = Mapper.Map<IEnumerable<ProductImage>, IEnumerable<ProductImageViewModel>>(model);
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Create(HttpRequestMessage request, ProductImageViewModel productImageVm)
        {
            if (ModelState.IsValid)
            {
                var model = new ProductImage();
                try
                {
                    model.UpdateProductImage(productImageVm);
                    _productImageService.Add(model);
                    _productImageService.Save();
                    return request.CreateResponse(HttpStatusCode.OK, productImageVm);
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

        [HttpDelete]
        [Route("delete")]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            _productImageService.Delete(id);
            _productImageService.Save();
            return request.CreateResponse(HttpStatusCode.OK, id);
        }
    }
}