using AutoMapper;
using System.Collections.Generic;
using System.Web.Mvc;
using TMDT.Service;
using TMDT.Web.Models.Common;
using TMDT.Model.Models;
using TMDT.Web.Models;
using TMDT.Common;
using System.Linq;
using System;
using Microsoft.AspNet.Identity;

namespace TMDT.Web.Controllers
{
    public class HomeController : Controller
    {
        IMenuService _menuService;
        ICommonService _commonService;
        IProductService _productService;
        IAnnouncementService _announcementService;
        public HomeController(IMenuService menuService, ICommonService commonService, IProductService productService, IAnnouncementService announcementService)
        {
            _menuService = menuService;
            _commonService = commonService;
            _productService = productService;
            _announcementService = announcementService;
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Footer()
        {
            return PartialView();
        }
        public ActionResult Header()
        {
            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            if (cart == null)
            {
                Session["count"] =0;
            }
            else
            {
                Session["count"] = Convert.ToInt32(cart.Count);
            }
            if (Request.IsAuthenticated)
            {
                int count = _announcementService.CountByUserId(User.Identity.GetUserId());
                ViewBag.Count = count;
                var announ = _announcementService.GetListByUserId(User.Identity.GetUserId(), 5);
                var announVm = Mapper.Map<List<Announcement>, List<AnnouncementViewModel>>(announ);
                ViewBag.ThongBao = announVm;
            }
            var model = _menuService.GetAll();
            var modelVm = Mapper.Map<IEnumerable<TMDT.Model.Models.Menu>, IEnumerable<MenuViewModel>>(model);
            return PartialView(modelVm);
        }

        public ActionResult Category()
        {
            var model = _productService.GetHotProduct(6);
            var modelVm = Mapper.Map<IEnumerable<Product>,IEnumerable<ProductViewModel>>(model);
            return PartialView(modelVm);
        }

        public ActionResult Slide()
        {
            var model = _commonService.GetAllSlide();
            var modelVm = Mapper.Map<IEnumerable<Slide>, IEnumerable<SlideViewModel>>(model);
            return PartialView(modelVm);
        }
       
    }
}