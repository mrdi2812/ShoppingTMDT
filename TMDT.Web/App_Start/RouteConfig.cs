using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TMDT.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*botdetect}", new { botdetect = @"(.*)BotDetectCaptcha\.ashx" });
            routes.MapRoute(
            name: "Contact",
            url: "lien-he.html",
            defaults: new { controller = "Contact", action = "Index", id = UrlParameter.Optional },
            namespaces: new string[] { "TMDT.Web.Controllers" }
           );
            routes.MapRoute(
             name: "Search",
             url: "tim-kiem.html",
             defaults: new { controller = "Product", action = "Search", id = UrlParameter.Optional },
             namespaces: new string[] { "TMDT.Web.Controllers" }
            );
            routes.MapRoute(
            name: "Page",
            url: "trang/{alias}.html",
            defaults: new { controller = "Page", action = "Index", alias = UrlParameter.Optional },
            namespaces: new string[] { "TMDT.Web.Controllers" }
           );

            routes.MapRoute(
            name: "Login",
            url: "dang-nhap.html",
            defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
            namespaces: new string[] { "TMDT.Web.Controllers" }
            );

            routes.MapRoute(
               name: "Register",
               url: "dang-ky.html",
               defaults: new { controller = "Account", action = "Register", id = UrlParameter.Optional },
               namespaces: new string[] { "TMDT.Web.Controllers" }
               );
            routes.MapRoute(
               name: "ChangePassword",
               url: "doi-mat-khau.html",
               defaults: new { controller = "Account", action = "ChangePassword", id = UrlParameter.Optional },
               namespaces: new string[] { "TMDT.Web.Controllers" }
               );
            routes.MapRoute(
               name: "ForgotPassword",
               url: "dat-lai-mat-khau.html",
               defaults: new { controller = "Account", action = "ForgotPassword", id = UrlParameter.Optional },
               namespaces: new string[] { "TMDT.Web.Controllers" }
               );
            routes.MapRoute(
               name: "ResetPassword",
               url: "quen-mat-khau.html",
               defaults: new { controller = "Account", action = "ResetPassword", id = UrlParameter.Optional },
               namespaces: new string[] { "TMDT.Web.Controllers" }
               );
            routes.MapRoute(
               name: "Cart",
               url: "gio-hang.html",
               defaults: new { controller = "ShoppingCart", action = "Index", id = UrlParameter.Optional },
               namespaces: new string[] { "TMDT.Web.Controllers" }
               );
            routes.MapRoute(
               name: "CheckOut",
               url: "thanh-toan.html",
               defaults: new { controller = "ShoppingCart", action = "CheckOut", id = UrlParameter.Optional },
               namespaces: new string[] { "TMDT.Web.Controllers" }
               );
            routes.MapRoute(
             name: "Confirm",
             url: "confirm.html",
             defaults: new { controller = "Account", action = "Confirm", id = UrlParameter.Optional },
             namespaces: new string[] { "TMDT.Web.Controllers" }
             );
            routes.MapRoute(
            name: "SentEmailConfirm",
            url: "sent-confirm-email.html",
            defaults: new { controller = "Account", action = "SentEmailConfirm", id = UrlParameter.Optional },
            namespaces: new string[] { "TMDT.Web.Controllers" }
            );
            routes.MapRoute(
             name: "ConfirmEmail",
             url: "confirm-email.html",
             defaults: new { controller = "Account", action = "ConfirmEmail", id = UrlParameter.Optional },
             namespaces: new string[] { "TMDT.Web.Controllers" }
             );
            routes.MapRoute(
             name: "UserInfo",
             url: "tai-khoan.html",
             defaults: new { controller = "Account", action = "Info", id = UrlParameter.Optional },
             namespaces: new string[] { "TMDT.Web.Controllers" }
             );
            routes.MapRoute(
            name: "Product Category",
            url: "{alias}.pc-{id}.html",
            defaults: new { controller = "Product", action = "Index", id = UrlParameter.Optional },
            namespaces: new string[] { "TMDT.Web.Controllers" }
            );
            routes.MapRoute(
            name: "Product",
            url: "{alias}.p-{id}.html",
            defaults: new { controller = "Product", action = "Detail", id = UrlParameter.Optional },
            namespaces: new string[] { "TMDT.Web.Controllers" }
            );
            routes.MapRoute(
            name: "Post",
            url: "{alias}.pt-{postId}.html",
            defaults: new { controller = "Post", action = "Detail", id = UrlParameter.Optional },
            namespaces: new string[] { "TMDT.Web.Controllers" }
            );
            routes.MapRoute(
           name: "PostTagList",
           url: "post-tag/{tagId}.html",
           defaults: new { controller = "Post", action = "ListPostByTag", tagId = UrlParameter.Optional },
           namespaces: new string[] { "TMDT.Web.Controllers" }
           );
            routes.MapRoute(
            name: "TagList",
            url: "tag/{tagId}.html",
            defaults: new { controller = "Product", action = "ListByTag", tagId = UrlParameter.Optional },
            namespaces: new string[] { "TMDT.Web.Controllers" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "TMDT.Web.Controllers" }
            );         

        }
    }
}
