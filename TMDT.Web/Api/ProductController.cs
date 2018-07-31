using AutoMapper;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using TMDT.Common;
using TMDT.Model.Models;
using TMDT.Service;
using TMDT.Web.Infrastructure.Core;
using TMDT.Web.Infrastructure.Extensions;
using TMDT.Web.Models;
using TMDT.Web.Models.Common;
using System.Runtime.Serialization;
using Antlr.Runtime.Misc;
using TMDT.Web.Providers;

namespace TMDT.Web.Api
{
    [RoutePrefix("api/product")]
    [Authorize]
    public class ProductController : ApiControllerBase
    {
        private IProductService _productService;

        public ProductController(IErrorService errorService, IProductService productService) : base(errorService)
        {
            _productService = productService;
        }

        [Route("getallparents")]
        [HttpGet]
        [Permission(Action = "Read", Function = "USER")]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _productService.GetAll();
                var modelVm = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(model);
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }

        [Route("gettags")]
        [HttpGet]
        [Permission(Action = "Read", Function = "USER")]
        public HttpResponseMessage GetTags(HttpRequestMessage request, string text)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _productService.GetListProductTag(text);

                var responseData = Mapper.Map<IEnumerable<Tag>, IEnumerable<TagViewModel>>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
            
        }

        [Route("detail/{id:int}")]
        [HttpGet]
        [Permission(Action = "Read", Function = "USER")]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _productService.GetById(id);

                var modelVm = Mapper.Map<Product, ProductViewModel>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, modelVm);

                return response;
            });
        }

        [Route("getall")]
        [HttpGet]
        [Permission(Action = "Read", Function = "USER")]
        public HttpResponseMessage GetAll(HttpRequestMessage request, int? categoryId, string keyword, int page, int pageSize = 20)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _productService.GetAll(categoryId, keyword);
                int totalRow = 0;
                model = model.OrderByDescending(x => x.CreatedDate).Skip(pageSize * (page - 1)).Take(pageSize);
                var modelVm = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(model);
                totalRow = model.Count();
                PaginationSet<ProductViewModel> pageSet = new PaginationSet<ProductViewModel>()
                {
                    PageIndex = page,
                    PageSize = pageSize,
                    TotalRows = totalRow,
                    Items = modelVm
                };
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }

        [Route("add")]
        [HttpPost]
        [Permission(Action = "Create", Function = "USER")]
        public HttpResponseMessage Create(HttpRequestMessage request, ProductViewModel productVm)
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
                    var newProduct = new Product();
                    newProduct.UpdateProduct(productVm);
                    newProduct.CreatedDate = DateTime.Now;
                    newProduct.CreatedBy = User.Identity.Name;
                    _productService.Add(newProduct);
                    _productService.Save();

                    var responseData = Mapper.Map<Product, ProductViewModel>(newProduct);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("update")]
        [HttpPut]
        [Permission(Action = "Update", Function = "USER")]
        public HttpResponseMessage Update(HttpRequestMessage request, ProductViewModel productVm)
        {
            return CreateHttpResponse(request, () =>
            {
                if (ModelState.IsValid)
                {
                    var product = _productService.GetById(productVm.ID);
                    product.UpdateProduct(productVm);
                    product.UpdatedDate = DateTime.Now;
                    _productService.Update(product);
                    _productService.Save();
                    var result = Mapper.Map<Product, ProductViewModel>(product);
                    var response = request.CreateResponse(HttpStatusCode.Created, result);
                    return response;
                }
                else
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            });
        }

        [Route("delete")]
        [HttpDelete]
        [Permission(Action = "Delete", Function = "USER")]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var oldProductCategory = _productService.Delete(id);
                _productService.Save();
                var responseData = Mapper.Map<Product, ProductViewModel>(oldProductCategory);
                response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        [Permission(Action = "Delete", Function = "USER")]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string checkedProducts)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var listProductCategory = new JavaScriptSerializer().Deserialize<List<int>>(checkedProducts);
                foreach (var item in listProductCategory)
                {
                    _productService.Delete(item);
                }
                _productService.Save();
                response = request.CreateResponse(HttpStatusCode.OK, listProductCategory.Count);
                return response;
            });
        }

        [Route("import")]
        [HttpPost]
        public async Task<HttpResponseMessage> Import()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "Định dạng không được server hỗ trợ");
            }

            var root = HttpContext.Current.Server.MapPath("~/UploadedFiles/Excels");
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            var provider = new MultipartFormDataStreamProvider(root);
            var result = await Request.Content.ReadAsMultipartAsync(provider);
            //do stuff with files if you wish
            if (result.FormData["categoryId"] == null)
            {
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bạn chưa chọn danh mục sản phẩm.");
            }

            //Upload files
            int addedCount = 0;
            int categoryId = 0;
            int.TryParse(result.FormData["categoryId"], out categoryId);
            foreach (MultipartFileData fileData in result.FileData)
            {
                if (string.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName))
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Yêu cầu không đúng định dạng");
                }
                string fileName = fileData.Headers.ContentDisposition.FileName;
                if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                {
                    fileName = fileName.Trim('"');
                }
                if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                {
                    fileName = Path.GetFileName(fileName);
                }

                var fullPath = Path.Combine(root, fileName);
                File.Copy(fileData.LocalFileName, fullPath, true);

                //insert to DB
                var listProduct = this.ReadProductFromExcel(fullPath, categoryId);
                if (listProduct.Count > 0)
                {
                    foreach (var product in listProduct)
                    {
                        _productService.Add(product);
                        addedCount++;
                    }
                    _productService.Save();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Đã nhập thành công " + addedCount + " sản phẩm thành công.");
        }

        [HttpGet]
        [Route("ExportXls")]
        public async Task<HttpResponseMessage> ExportXls(HttpRequestMessage request, string filter = null)
        {
            string fileName = string.Concat("Product_" + DateTime.Now.ToString("yyyyMMddhhmmsss") + ".xlsx");
            var folderReport = ConfigHelper.GetByKey("ReportFolder");
            string filePath = HttpContext.Current.Server.MapPath(folderReport);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string fullPath = Path.Combine(filePath, fileName);
            try
            {
                var data = _productService.GetListProduct(filter).ToList();
                await ReportHelper.GenerateXls(data, fullPath);
                return request.CreateErrorResponse(HttpStatusCode.OK, Path.Combine(folderReport, fileName));
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("ExportPdf")]
        public async Task<HttpResponseMessage> ExportPdf(HttpRequestMessage request, int id)
        {
            string fileName = string.Concat("Product" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".pdf");
            var folderReport = ConfigHelper.GetByKey("ReportFolder");
            string filePath = HttpContext.Current.Server.MapPath(folderReport);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string fullPath = Path.Combine(filePath, fileName);
            try
            {
                var template = File.ReadAllText(HttpContext.Current.Server.MapPath("/Assets/admin/templates/product-detail.html"));
                var replaces = new Dictionary<string, string>();
                var product = _productService.GetById(id);

                replaces.Add("{{ProductName}}", product.Name);
                replaces.Add("{{Price}}", product.Price.ToString("N0"));
                replaces.Add("{{Description}}", product.Description);
                replaces.Add("{{Warranty}}", product.Warranty + " tháng");

                template = template.Parse(replaces);

                await ReportHelper.GeneratePdf(template, fullPath);
                return request.CreateErrorResponse(HttpStatusCode.OK, Path.Combine(folderReport, fileName));
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        private List<Product> ReadProductFromExcel(string fullPath, int categoryId)
        {
            using (var package = new ExcelPackage(new FileInfo(fullPath)))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                List<Product> listProduct = new List<Product>();
                ProductViewModel productViewModel;
                Product product;

                decimal originalPrice = 0;
                decimal price = 0;
                decimal promotionPrice;

                bool status = false;
                bool showHome = false;
                bool isHot = false;
                int warranty;

                for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                {
                    productViewModel = new ProductViewModel();
                    product = new Product();

                    productViewModel.Name = workSheet.Cells[i, 1].Value.ToString();
                    productViewModel.Alias = StringHelper.ToUnsignString(productViewModel.Name);
                    productViewModel.Description = workSheet.Cells[i, 2].Value.ToString();

                    if (int.TryParse(workSheet.Cells[i, 3].Value.ToString(), out warranty))
                    {
                        productViewModel.Warranty = warranty;
                    }

                    decimal.TryParse(workSheet.Cells[i, 4].Value.ToString().Replace(",", ""), out originalPrice);
                    productViewModel.OriginalPrice = originalPrice;

                    decimal.TryParse(workSheet.Cells[i, 5].Value.ToString().Replace(",", ""), out price);
                    productViewModel.Price = price;

                    if (decimal.TryParse(workSheet.Cells[i, 6].Value.ToString(), out promotionPrice))
                    {
                        productViewModel.PromotionPrice = promotionPrice;
                    }

                    productViewModel.Content = workSheet.Cells[i, 7].Value.ToString();
                    productViewModel.MetaKeyword = workSheet.Cells[i, 8].Value.ToString();
                    productViewModel.MetaDescription = workSheet.Cells[i, 9].Value.ToString();

                    productViewModel.CategoryID = categoryId;

                    bool.TryParse(workSheet.Cells[i, 10].Value.ToString(), out status);
                    productViewModel.Status = status;

                    bool.TryParse(workSheet.Cells[i, 11].Value.ToString(), out showHome);
                    productViewModel.HomeFlag = showHome;

                    bool.TryParse(workSheet.Cells[i, 12].Value.ToString(), out isHot);
                    productViewModel.HotFlag = isHot;

                    product.UpdateProduct(productViewModel);
                    listProduct.Add(product);
                }
                return listProduct;
            }
        }
    }
}