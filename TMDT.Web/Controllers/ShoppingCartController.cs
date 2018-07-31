using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TMDT.Common;
using TMDT.Model.Models;
using TMDT.Service;
using TMDT.Web.App_Start;
using TMDT.Web.Infrastructure.Extensions;
using TMDT.Web.Models;
using TMDT.Web.Models.Common;

namespace TMDT.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart
        IProductService _productService;
        IOrderService _orderService;
        IProductQuantityService _productQuantityService;
        private ApplicationUserManager _userManager;
        public ShoppingCartController(IProductService productService, IOrderService orderService, ApplicationUserManager userManager,
            IProductQuantityService productQuantityService)
        {
            _productService = productService;
            _orderService = orderService;
            _userManager = userManager;
            _productQuantityService = productQuantityService;
        }
        public ActionResult Index()
        {
            if (Session[CommonConstants.SessionCart] == null)
                Session[CommonConstants.SessionCart] = new List<ShoppingCartViewModel>();           
            return View();
        }
        public JsonResult GetAll()
        {
            if (Session[CommonConstants.SessionCart] == null)
                Session[CommonConstants.SessionCart] = new List<ShoppingCartViewModel>();
            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            return Json(new
            {
                data = cart,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetDetail(int productId,string category)
        {
            var cartSesstion = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            foreach (var item in cartSesstion)
            {
                if (item.ProductId == productId && item.Category == category)
                {
                    return Json(new
                    {
                        data = item,
                        status = true
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                status = true
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Add(int productId,string name,int colorId,int sizeId,int soluong)
        {
            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            var product = _productService.GetById(productId);
            if (cart == null)
            {
                cart = new List<ShoppingCartViewModel>();
            }
            if (cart.Any(x => x.ProductId == productId))
            {
                foreach (var item in cart)
                {
                    if (item.ProductId == productId&&item.Category==name)
                    {
                        item.Quantity += soluong;
                    }
                }
            }
            else
            {
                ShoppingCartViewModel newItem = new ShoppingCartViewModel();
                newItem.ProductId = productId;
                newItem.Product = Mapper.Map<Product, ProductViewModel>(product);
                newItem.Quantity = soluong;
                newItem.Category =name;
                newItem.ColorId = colorId;
                newItem.SizeId = sizeId;              
                cart.Add(newItem);
                Session["count"] = Convert.ToInt32(Session["count"]) + 1;
            }

            Session[CommonConstants.SessionCart] = cart;
            return Json(new
            {
                data = Session["count"],
                status = true
            },JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Update(string cartData)
        {
            var cartViewModel = new JavaScriptSerializer().Deserialize<List<ShoppingCartViewModel>>(cartData);
            var cartSesstion = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            foreach (var item in cartSesstion)
            {
                foreach (var item1 in cartViewModel)
                {
                    if (item.ProductId == item1.ProductId&&item.Category==item1.Category)
                    {
                        item.Quantity = item1.Quantity;
                    }
                }
            }
            Session[CommonConstants.SessionCart] = cartSesstion;
            return Json(new
            {
                status = true
            });
        }
        [HttpPost]
        public JsonResult UpdateItem(int productId,string name,int quantity)
        {
            var cartSesstion = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            foreach (var item in cartSesstion)
            {
                    if (item.ProductId == productId && item.Category == name)
                    {
                    item.Quantity = quantity;
                    }
            }
            Session[CommonConstants.SessionCart] = cartSesstion;
            return Json(new
            {
                status = true
            });
        }
        [HttpPost]
        public JsonResult DeleteItem(int productId)
        {
            var cartSession = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            if (cartSession != null)
            {
                cartSession.RemoveAll(x => x.ProductId == productId);
                Session[CommonConstants.SessionCart] = cartSession;
                Session["count"] = Convert.ToInt32(Session["count"]) - 1;
                return Json(new
                {
                    data = Session["count"],
                    status = true
                });
            }
            return Json(new
            {
                status = false
            });
        }

        [HttpPost]
        public JsonResult DeteleAll()
        {
            Session[CommonConstants.SessionCart] = new List<ShoppingCartViewModel>();
            return Json(new
            {
                data = 0,
                status = true
            });
        }
        public JsonResult GetUser()
        {
            if (Request.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var user = _userManager.FindById(userId);
                return Json(new
                {
                    data = user,
                    status = true
                });
            }
            return Json(new
            {
                status = false
            });
        }
        public JsonResult CreateOrder(string orderVM)
        {
            var order = new JavaScriptSerializer().Deserialize<OrderViewModel>(orderVM);
            var orderNew = new Order();

            orderNew.UpdateOrder(order);

            if (Request.IsAuthenticated)
            {
                orderNew.CustomerId = User.Identity.GetUserId();
                orderNew.CreatedBy = User.Identity.GetUserName();
            }
            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            bool isEnough = true;
            foreach (var item in cart)
            {
                var detail = new OrderDetail();
                detail.ProductID = item.ProductId;
                detail.Quantity = item.Quantity;
                detail.Price = item.Product.Price;
                detail.ColorId = item.ColorId;
                detail.SizeId = item.SizeId;
                orderDetails.Add(detail);
                isEnough = _productQuantityService.SellProduct(item.ProductId,item.SizeId,item.ColorId,item.Quantity);
                break;
            }
            if (isEnough)
            {
                var orderVm = _orderService.Add(orderNew);
                foreach(var item in orderDetails)
                {
                    item.OrderID = orderVm.ID;
                    _orderService.CreateDetail(item);
                }                
                _productService.Save();
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false,
                    message = "Không đủ hàng."
                });
            }
        }
    }
}