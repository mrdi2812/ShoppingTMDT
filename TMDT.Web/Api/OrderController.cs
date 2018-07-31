using AutoMapper;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TMDT.Common;
using TMDT.Service;
using TMDT.Model.Models;
using TMDT.Web.Infrastructure.Core;
using TMDT.Web.Providers;
using TMDT.Web.Models;
using TMDT.Web.Infrastructure.Extensions;
using TMDT.Web.SignalR;

namespace TMDT.Web.Api
{
    [RoutePrefix("api/Order")]
    public class OrderController : ApiControllerBase
    {
        private IOrderService _orderService;
        public OrderController(IErrorService errorService, IOrderService orderService) : base(errorService)
        {
            _orderService = orderService;
        }
        [Route("getlistpaging")]
        [HttpGet]
        [Permission(Action = "Read", Function = "ORDER")]
        public HttpResponseMessage GetListPaging(HttpRequestMessage request, string startDate, string endDate,
           string customerName, string paymentStatus, int page, int pageSize, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                int totalRow = 0;
                var model = _orderService.GetListByDate(startDate, endDate, customerName, paymentStatus, page, pageSize, out totalRow);
                var modelVm = Mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(model);
                PaginationSet<OrderViewModel> paged = new PaginationSet<OrderViewModel>
                {
                    PageIndex = page,
                    PageSize = pageSize,
                    TotalRows = totalRow,
                    Items = modelVm
                };
                response = request.CreateResponse(HttpStatusCode.OK, paged);
                return response;
            });
        }
        [Route("detail/{id}")]
        [HttpGet]
        //[Authorize(Roles = "ViewUser")]
        [Permission(Action = "Read", Function = "ORDER")]
        public HttpResponseMessage Details(HttpRequestMessage request, int id)
        {
            if (id == 0)
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị.");
            }
            else
            {
                var model = _orderService.GetDetail(id);
                if (model == null)
                {
                    return request.CreateErrorResponse(HttpStatusCode.NoContent, "No data");
                }
                var modelVm = Mapper.Map<Order, OrderViewModel>(model);
                return request.CreateResponse(HttpStatusCode.OK, modelVm);
            }
        }
        [Route("getalldetails/{id}")]
        [HttpGet]
        [Permission(Action = "Read", Function = "ORDER")]
        //[Authorize(Roles = "ViewUser")]
        public HttpResponseMessage GetOrderDetails(HttpRequestMessage request, int id)
        {
            if (id == 0)
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị.");
            }
            else
            {
                var model = _orderService.GetOrderDetails(id);
                if (model == null)
                {
                    return request.CreateErrorResponse(HttpStatusCode.NoContent, "No data");
                }
                var modelVm = Mapper.Map<IEnumerable<OrderDetail>, IEnumerable<OrderDetailViewModel>>(model);
                return request.CreateResponse(HttpStatusCode.OK, modelVm);
            }
        }
        [HttpPost]
        [Route("add")]
        //[Authorize(Roles = "AddUser")]
        [Permission(Action = "Create", Function = "USER")]
        public HttpResponseMessage Create(HttpRequestMessage request, OrderViewModel orderVm)
        {
            if (ModelState.IsValid)
            {
                var order = new Order();
                order.UpdateOrder(orderVm);
                order.CreatedBy = User.Identity.Name;
                order.CreatedDate =DateTime.Parse(DateTime.Now.ToShortDateString());
                try
                {
                    var listorderDetail = new List<OrderDetail>();
                    foreach(var item in orderVm.OrderDetails)
                    {
                        listorderDetail.Add(new OrderDetail()
                        {
                            OrderID = item.OrderID,
                            ColorId = item.ColorId,
                            SizeId = item.SizeId,
                            ProductID = item.ProductID,
                            Price = item.Price,
                            Quantity = item.Quantity
                        });
                    }
                    order.OrderDetails = listorderDetail;
                    var result = _orderService.Add(order);
                    _orderService.Save();
                    var model = Mapper.Map<Order,OrderViewModel>(result);
                    return request.CreateResponse(HttpStatusCode.OK,model);
                }
                catch (Exception ex)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                }
                
            }
            else
            {
                return request.CreateResponse(HttpStatusCode.BadRequest,ModelState);
            }
        }
        [HttpPut]
        [Route("updateStatus")]
        //[Authorize(Roles = "UpdateUser")]
        [Permission(Action = "Update", Function = "ORDER")]
        public HttpResponseMessage UpdateStatus(HttpRequestMessage request, int orderId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _orderService.UpdateStatus(orderId);
                    return request.CreateResponse(HttpStatusCode.OK, orderId);
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
        //[Authorize(Roles = "UpdateUser")]
        [Permission(Action = "Delete", Function = "ORDER")]
        public HttpResponseMessage Delete(HttpRequestMessage request, int orderId)
        {
            if (ModelState.IsValid)
            {
                try
                {            
                    var modeldetail = _orderService.GetOrderDetails(orderId);
                    foreach(var item in modeldetail)
                    {
                        _orderService.DeleteDetail(item.ProductID, item.OrderID, item.ColorId, item.SizeId);
                    }
                    _orderService.Delete(orderId);
                    return request.CreateResponse(HttpStatusCode.OK, orderId);
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
        [Route("exportExcel/{id}")]
        [HttpGet]
        public HttpResponseMessage ExportOrder(HttpRequestMessage request, int id)
        {
            var folderReport = ConfigHelper.GetByKey("ReportFolder");
            string filePath = HttpContext.Current.Server.MapPath(folderReport);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string documentName = GenerateOrder(id);
            if (!string.IsNullOrEmpty(documentName))
            {
                return request.CreateErrorResponse(HttpStatusCode.OK, folderReport + "/" + documentName);
            }
            else
            {
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error export");

            }

            // If something fails or somebody calls invalid URI, throw error.
        }
        #region Export to Excel
        private string GenerateOrder(int orderId)
        {
            var folderReport = ConfigHelper.GetByKey("ReportFolder");
            string filePath = HttpContext.Current.Server.MapPath(folderReport);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            // Template File
            string templateDocument =
                    HttpContext.Current.Server.MapPath("~/Templates/OrderTemplate.xlsx");
            string documentName = string.Format("Order-{0}-{1}.xlsx", orderId, DateTime.Now.ToString("yyyyMMddhhmmsss"));
            string fullPath = Path.Combine(filePath, documentName);
            // Results Output
            MemoryStream output = new MemoryStream();
            try
            {
                // Read Template
                using (FileStream templateDocumentStream = File.OpenRead(templateDocument))
                {
                    // Create Excel EPPlus Package based on template stream
                    using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                    {
                        // Grab the sheet with the template, sheet name is "BOL".
                        ExcelWorksheet sheet = package.Workbook.Worksheets["TMDTOrder"];
                        // Data Acces, load order header data.
                        var order = _orderService.GetDetail(orderId);

                        // Insert customer data into template
                        sheet.Cells[4, 1].Value = "Tên khách hàng: " + order.CustomerName;
                        sheet.Cells[5, 1].Value = "Địa chỉ: " + order.CustomerAddress;
                        sheet.Cells[6, 1].Value = "Điện thoại: " + order.CustomerMobile;
                        // Start Row for Detail Rows
                        int rowIndex = 9;

                        // load order details
                        var orderDetails = _orderService.GetOrderDetails(orderId);
                        int count = 1;
                        foreach (var orderDetail in orderDetails)
                        {
                            // Cell 1, Carton Count
                            sheet.Cells[rowIndex, 1].Value = count.ToString();
                            // Cell 2, Order Number (Outline around columns 2-7 make it look like 1 column)
                            sheet.Cells[rowIndex, 2].Value = orderDetail.Product.Name;
                            // Cell 8, Weight in LBS (convert KG to LBS, and rounding to whole number)
                            sheet.Cells[rowIndex, 3].Value = orderDetail.Quantity.ToString();

                            sheet.Cells[rowIndex, 4].Value = orderDetail.Price.ToString("N0");
                            sheet.Cells[rowIndex, 5].Value = (orderDetail.Price * orderDetail.Quantity).ToString("N0");
                            // Increment Row Counter
                            rowIndex++;
                            count++;
                        }
                        double total = (double)(orderDetails.Sum(x => x.Quantity * x.Price));
                        sheet.Cells[24, 5].Value = total.ToString("N0");

                        var numberWord = "Thành tiền (viết bằng chữ): " + NumberHelper.ToString(total);
                        sheet.Cells[26, 1].Value = numberWord;
                        if (order.CreatedDate.HasValue)
                        {
                            var date = order.CreatedDate.Value;
                            sheet.Cells[28, 3].Value = "Ngày " + date.Day + " tháng " + date.Month + " năm " + date.Year;

                        }
                        package.SaveAs(new FileInfo(fullPath));
                    }
                    return documentName;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }
        #endregion

    }
}
