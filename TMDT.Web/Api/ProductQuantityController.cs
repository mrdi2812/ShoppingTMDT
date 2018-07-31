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
    [RoutePrefix("api/productQuantity")]
    public class ProductQuantityController : ApiControllerBase
    {
        private IProductQuantityService _productQuantityService;

        public ProductQuantityController(IErrorService errorService, IProductQuantityService productQuantityService) : base(errorService)
        {
            _productQuantityService = productQuantityService;
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request, int productId, int? sizeId, int? colorId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _productQuantityService.GetAll(productId, sizeId, colorId);
                var modelVm = Mapper.Map<IEnumerable<ProductQuantity>, IEnumerable<ProductQuantityViewModel>>(model);
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Create(HttpRequestMessage request, ProductQuantityViewModel productQuantityVm)
        {
            if (ModelState.IsValid)
            {
                var model = new ProductQuantity();
                try
                {
                    if (_productQuantityService.CheckExist(productQuantityVm.ProductId, productQuantityVm.SizeId, productQuantityVm.ColorId))
                    {
                        return request.CreateErrorResponse(HttpStatusCode.BadRequest, "Màu sắc kích thước cho sản phẩm này đã tồn tại");
                    }
                    else
                    {
                        model.UpdateProductQuantity(productQuantityVm);
                        _productQuantityService.Add(model);
                        _productQuantityService.Save();
                        return request.CreateResponse(HttpStatusCode.OK, productQuantityVm);
                    }
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

        [HttpDelete]
        [Route("delete")]
        public HttpResponseMessage Delete(HttpRequestMessage request, int productId, int colorId, int sizeId)
        {
            _productQuantityService.Delete(productId, colorId, sizeId);
            _productQuantityService.Save();
            return request.CreateResponse(HttpStatusCode.OK, "Xóa thành công");
        }

        [Route("getcolors")]
        [HttpGet]
        public HttpResponseMessage GetColors(HttpRequestMessage request,string filter)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _productQuantityService.GetListColor(filter);

                var modelVm = Mapper.Map<IEnumerable<Color>, IEnumerable<ColorViewModel>>(model);

                response = request.CreateResponse(HttpStatusCode.OK, modelVm);

                return response;
            });
        }

        [Route("getsizes")]
        [HttpGet]
        public HttpResponseMessage GetSizes(HttpRequestMessage request,string filter)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _productQuantityService.GetListSize(filter);

                var modelVm = Mapper.Map<IEnumerable<Size>, IEnumerable<SizeViewModel>>(model);

                response = request.CreateResponse(HttpStatusCode.OK, modelVm);

                return response;
            });
        }
        [HttpPost]
        [Route("addcolor")]
        public HttpResponseMessage CreateColor(HttpRequestMessage request, ColorViewModel colorVm)
        {
            if (ModelState.IsValid)
            {
                var model = new Color();
                try
                {
                        model.UpdateColor(colorVm);
                        _productQuantityService.AddColor(model);
                        _productQuantityService.Save();
                        return request.CreateResponse(HttpStatusCode.OK, colorVm);
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
        [HttpPost]
        [Route("addsize")]
        public HttpResponseMessage CreateSize(HttpRequestMessage request, SizeViewModel sizeVm)
        {
            if (ModelState.IsValid)
            {
                var model = new Size();
                try
                {
                    model.UpdateSize(sizeVm);
                    _productQuantityService.AddSize(model);
                    _productQuantityService.Save();
                    return request.CreateResponse(HttpStatusCode.OK, sizeVm);
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
        [HttpDelete]
        [Route("deletecolor")]
        public HttpResponseMessage DeleteColor(HttpRequestMessage request,int colorId)
        {
            _productQuantityService.DeleteColor(colorId);
            _productQuantityService.Save();
            return request.CreateResponse(HttpStatusCode.OK, "Xóa thành công");
        }
        [HttpDelete]
        [Route("deletesize")]
        public HttpResponseMessage DeleteSize(HttpRequestMessage request,int sizeId)
        {
            _productQuantityService.DeleteSize( sizeId);
            _productQuantityService.Save();
            return request.CreateResponse(HttpStatusCode.OK, "Xóa thành công");
        }
    }
}