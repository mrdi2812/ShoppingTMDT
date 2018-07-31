using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using TMDT.Model.Models;
using TMDT.Service;
using TMDT.Web.Infrastructure.Core;
using TMDT.Web.Infrastructure.Extensions;
using TMDT.Web.Models;

namespace TMDT.Web.Api
{
    [RoutePrefix("api/productCategory")]
    [Authorize]
    public class ProductCategoryController : ApiControllerBase
    {
        private IProductCategoryService _productCategoryService;

        public ProductCategoryController(IErrorService errorService, IProductCategoryService productCategoryService) : base(errorService)
        {
            _productCategoryService = productCategoryService;
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request,string filter)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _productCategoryService.GetAll(filter);
                var modelVm = Mapper.Map<IEnumerable<ProductCategory>, IEnumerable<ProductCategoryViewModel>>(model);
                return request.CreateResponse(HttpStatusCode.OK, modelVm);
            });
        }

        [Route("getallhierachy")]
        [HttpGet]
        public HttpResponseMessage GetAllHierachy(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                return request.CreateResponse(HttpStatusCode.OK, GetCategoryViewModel());
            });
        }

        [Route("detail/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            if (id == 0)
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị");
            }
            var model = _productCategoryService.GetById(id);
            var modelVm = Mapper.Map<ProductCategory, ProductCategoryViewModel>(model);
            return request.CreateResponse(HttpStatusCode.OK, modelVm);
        }

   
        [Route("add")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Create(HttpRequestMessage request, ProductCategoryViewModel productCategoryVm)
        {
            if (ModelState.IsValid)
            {
                return CreateHttpResponse(request, () =>
                {
                    var model = new ProductCategory();
                    model.UpdateProductCategory(productCategoryVm);
                    model.CreatedDate = DateTime.Now;
                    _productCategoryService.Add(model);
                    _productCategoryService.Save();
                    var result = Mapper.Map<ProductCategory, ProductCategoryViewModel>(model);
                    return request.CreateResponse(HttpStatusCode.OK, result);
                });
            }
            else
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        [Route("update")]
        [HttpPut]
        public HttpResponseMessage Update(HttpRequestMessage request, ProductCategoryViewModel productCategoryVm)
        {
            if (ModelState.IsValid)
            {
                return CreateHttpResponse(request, () =>
                {
                    if (productCategoryVm.ID == productCategoryVm.ParentID)
                    {
                        return request.CreateErrorResponse(HttpStatusCode.BadRequest, "Danh mục này không thể làm danh mục con chính nó");
                    }
                    var model = _productCategoryService.GetById(productCategoryVm.ID);
                    model.UpdateProductCategory(productCategoryVm);
                    model.UpdatedDate = DateTime.Now;
                    _productCategoryService.Update(model);
                    try
                    {
                        _productCategoryService.Save();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                    var result = Mapper.Map<ProductCategory, ProductCategoryViewModel>(model);
                    return request.CreateResponse(HttpStatusCode.OK, result);
                });
            }
            else
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        [Route("delete")]
        [HttpDelete]
        [AllowAnonymous]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var oldProductCategory = _productCategoryService.Delete(id);
                _productCategoryService.Save();
                var responseData = Mapper.Map<ProductCategory, ProductCategoryViewModel>(oldProductCategory);
                response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        [AllowAnonymous]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string checkedProductCategories)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var listProductCategory = new JavaScriptSerializer().Deserialize<List<int>>(checkedProductCategories);
                    foreach (var item in listProductCategory)
                    {
                        _productCategoryService.Delete(item);
                    }

                    _productCategoryService.Save();

                    response = request.CreateResponse(HttpStatusCode.OK, listProductCategory.Count);
                }

                return response;
            });
        }

        //Load các ProductCategory cha có ParentId ==null
        private List<ProductCategoryViewModel> GetCategoryViewModel(long? selectedParent = null)
        {
            List<ProductCategoryViewModel> items = new List<ProductCategoryViewModel>();

            //get all of them from DB
            var allCategorys = _productCategoryService.GetAll();
            //get parent categories
            IEnumerable<ProductCategory> parentCategorys = allCategorys.Where(c => c.ParentID == null);

            foreach (var cat in parentCategorys)
            {
                //add the parent category to the item list
                items.Add(new ProductCategoryViewModel
                {
                    ID = cat.ID,
                    Name = cat.Name,
                    DisplayOrder = cat.DisplayOrder,
                    Status = cat.Status,
                    CreatedDate = cat.CreatedDate
                });
                //now get all its children (separate Category in case you need recursion)
                GetSubTree(allCategorys.ToList(), cat, items);
            }
            return items;
        }

        //Chia ProductCategory theo parentId truyền vào theo dạng tree
        private void GetSubTree(IList<ProductCategory> allCats, ProductCategory parent, IList<ProductCategoryViewModel> items)
        {
            var subCats = allCats.Where(c => c.ParentID == parent.ID);
            foreach (var cat in subCats)
            {
                //add this category
                items.Add(new ProductCategoryViewModel
                {
                    ID = cat.ID,
                    Name = parent.Name + " >> " + cat.Name,
                    DisplayOrder = cat.DisplayOrder,
                    Status = cat.Status,
                    CreatedDate = cat.CreatedDate
                });
                //recursive call in case your have a hierarchy more than 1 level deep
                GetSubTree(allCats, cat, items);
            }
        }
    }
}